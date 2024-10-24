﻿using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using static CreatureConfigSize.References;
using UnityEngine;
using System;

namespace CreatureConfigSize
{
    [HarmonyPatch]
    internal class CreatureConfigSize
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        [HarmonyPrefix]
        public static void PrePlayerStart()
        {
            //Add inventory size of the Reaper Leviathan when picking it up
            //CraftData.itemSizes.Add(TechType.ReaperLeviathan, new Vector2int(3, 3));

            //DEBUG!! Showcase what options are on or off
            logger.LogInfo($"All Pickupable = {config.AllowAllPickupable}");
            logger.LogInfo($"All WaterPark = {config.AllowAllWaterPark}");
        }

        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Awake))]
        [HarmonyPostfix] //Using LiveMixin because creatures in containment don't trigger Creature events (because their creature component is disabled)
        //Using .Awake to set up the placeholder WPC data, as by .Start it's pulling null references and breaking
        public static void PostLiveMixinAwake(Creature __instance)
        {
            GameObject creature = __instance.gameObject;
            TechType techType = CraftData.GetTechType(creature);
            logger.LogInfo($"(PostLiveMixinAwake) entity is {techType}");

            //If the creature can be in a WaterPark but isn't normally, we need to create a new blank placeholder WPC data at start
            //Otherwise it breaks, because they're null otherwise, on account of not normally having WPC component
            //NOTE!! Only limiting which creatures gets a placeholder WPC component by techType, not by size, because size is not calculated this early on; easier this way
            if (WaterParkReference.ContainsKey(techType))
            {
                //Ensure the creature has the WaterParkCreature component
                WaterParkCreature wpc = creature.EnsureComponent<WaterParkCreature>();

                //Create an empty WaterParkCreatureData for us to populate, if it's empty
                if (wpc.data == null)
                {
                    logger.LogInfo($"(PostLiveMixinAwake) WaterParkCreatureData of {techType} is null! Creating placeholder!");
                    wpc.data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
                }
            }
        }

        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Start))]
        [HarmonyPostfix] //Using LiveMixin because creatures in containment don't trigger Creature events (because their creature component is disabled)
        //Using .Start for everything else because size changes don't take effect if done in .Awake
        public static void PostLiveMixinStart(Creature __instance)
        {
            //NOTE!! This might exclude creatures we've seen not work with the command, like Blood Crawlers
            if(__instance.gameObject.GetComponent<Creature>() != null)
            {
                //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
                GameObject creature = __instance.gameObject;

                TechType techType = CraftData.GetTechType(creature);
                ErrorMessage.AddMessage($"Creature {techType} found");
                logger.LogInfo($"Creature {techType} found");

                //TODO!! Check for whether it's a baby or not and assign it the unused TechType, for my own sanity making checks in future
                if (techType == TechType.Reefback)
                {
                    logger.LogInfo($"{creature.name}");
                }

                
                if (techType != TechType.None)
                {
                    //Further check to make sure creature isn't a school of fish; they don't like being scaled up, though have the creature component
                    if (techType != TechType.HoopfishSchool)
                    {
                        //NOTE!! We apply them if their size is 1, as this is hopefully the baseline for many creatures
                        //NOTE 2!! Unfortunately, not all creatures start at size 1; notably small fish and Sea Treaders

                        //Maybe I create a component in code, to hold onto the value and then apply it after its loaded?
                        logger.LogInfo($"Creature Size = {GetSize(creature)}");
                        if (GetSize(creature) == 1)
                        {
                            //Generate a modifier based on the creature's size class, retrieved from the CreatureSizeReference array
                            float modifier = GetCreatureSizeModifier(techType);
                            logger.LogInfo($"Creature Modifier = {modifier}");

                            //Once we've retrieved the modifier, apply the change to size, by the modifier
                            SetSize(creature, modifier);

                            ErrorMessage.AddMessage($"Changed size of {techType} to {modifier}");
                        }

                        #region DEBUG!! Test for checking for creatures who's size isn't 1 set by the game
                        //If the creature value rounded has more than 1 decimal place, then it's not one set by me and needs changing
                        //ERROR!! This will not work longterm, because fauna growing up in the tank will falsely trigger this
                        decimal creatureSize = (decimal)GetSize(creature);
                        if ((Decimal.Round(creatureSize, 1) != creatureSize))
                        {
                            logger.LogInfo($"Size of {techType} was set by game; resizing.");
                            //Generate a modifier based on the creature's size class, retrieved from the CreatureSizeReference array
                            float modifier = GetCreatureSizeModifier(techType);
                            logger.LogInfo($"Creature Modifier = {modifier}");

                            //Once we've retrieved the modifier, apply the change to size, by the modifier
                            SetSize(creature, modifier);

                            ErrorMessage.AddMessage($"Changed size of {techType} to {modifier}");
                        }
                        #endregion

                        var size = GetSize(creature);
                        logger.LogInfo($"Creature Size After = {size}");

                        //Check whether the creature is eligible to be picked up (and have the Pickupable component) or not
                        CheckPickupableComponent(creature, size);

                        //Check whether the creature is eligible to be placed in alien containment up (and have the WPC component) or not
                        CheckWaterParkCreatureComponent(__instance.gameObject, size);
                    }
                }
                else
                {
                    logger.LogWarning($"Error! Creature {__instance.name} has no TechType!");
                }
            }
        }

        //Check whether the creature's size makes it eligible or not for the Pickupable component, and to add it or remove it
        public static bool CheckPickupableComponent(GameObject creature, float size)
        {
            //Generate techtype to check the dictionary for creature's entry
            TechType techType = CraftData.GetTechType(creature);

            if(PickupableReference.ContainsKey(techType))
            {
                //The size range within which a creature is made able to be picked up; component removed if outside of this range
                var (min, max) = PickupableReference[techType];

                //Whether the creature has a Pickupable component already or not
                bool componentExists = !(creature.GetComponent<Pickupable>() == null);

                //Whether the creature is in alien containment or not
                bool insideWaterPark = GetInsideWaterPark(creature);


                //Calculate the creature's original size modifier whether its current size or a larger size if the creature is in containment and has been shrunk to 60%
                float modifier;

                if (insideWaterPark)
                {
                    logger.LogWarning($"(Pickupable) {techType} is already inside alien containment! Calcuating original size modifier from current size!");

                    //By performing the calculation to get maxSize for WPC data (x * 0.6), but in reverse (x / 0.6), we get our old size back and original size modifier
                    modifier = size / 0.6f;

                }
                else
                {
                    logger.LogWarning($"(Pickupable) {techType} is not inside alien containment! Using current size for size modifier!");

                    //If the creature isn't in alien containment, this means its current size is equal to its size modifier
                    modifier = size;
                }


                //Use original size modifier to determine if eligible for pickupable component
                if ((modifier >= min && modifier <= max) || config.AllowAllPickupable)
                {
                    //If creature is eligible for the component and doesn't have one, add it and return true
                    if(!componentExists)
                    {
                        creature.AddComponent<Pickupable>();
                        return true;
                    }
                }
                else
                {
                    //If creature is ineligable for the component and has one, remove it (and will return false by default)
                    //NOTE!! If creature is in alien containment, we DO NOT remove this component; IsPickupableOutside is used instead to remove the component when the creature is placed outside
                    if(componentExists || insideWaterPark)
                    {
                        var component = creature.GetComponent<Pickupable>();
                        UnityEngine.Object.Destroy(component);
                    }
                }
            }
            else
            {
                ErrorMessage.AddError($"{techType} is not in the pickupable reference dictionary!");
            }

            //Return false if any of the if statements are false
            return false;
        }

        //TODO!! Look into hooking into when a creatures hatches from an egg, and set its size there randomly (can be outside the range of pickupable, but within the range of what will fit
        //in alien containment; this is how you can get bigger sizes in there, if you can't pick them up); they'll still be about 33% of their size in the tank though, so they grow as
        //newly hatched fish do

        //Check whether the creature's size makes it eligible or not for the WaterParkCreature component, and to add it or remove it
        public static bool CheckWaterParkCreatureComponent(GameObject creature, float size)
        {
            //Generate techtype to check the dictionary for creature's entry
            TechType techType = CraftData.GetTechType(creature);

            if(WaterParkReference.ContainsKey(techType))
            {
                var (min, max) = WaterParkReference[techType];


                //Calculate the creature's original size modifier whether its current size or a larger size if the creature is in containment and has been shrunk to 60%
                float modifier;

                if (GetInsideWaterPark(creature))
                {
                    logger.LogWarning($"(WaterParkCreature) {techType} is already inside alien containment! Calcuating original size modifier from current size!");

                    //By performing the calculation to get maxSize for WPC data (x * 0.6), but in reverse (x / 0.6), we get our old size back and original size modifier
                    modifier = size / 0.6f;

                }
                else
                {
                    logger.LogWarning($"(WaterParkCreature) {techType} is not inside alien containment! Using current size for size modifier!");
                    
                    //If the creature isn't in alien containment, this means its current size is equal to its size modifier
                    modifier = size;
                }


                if ((modifier >= min && modifier <= max) || config.AllowAllWaterPark)
                {
                    //NOTE!! NEED TO MAKE SURE THIS IS RUNNING WELL
                    //THIS MIGHT USELESS CODE!!

                    //Ensure the creature has the WaterParkCreature component
                    WaterParkCreature wpc = creature.EnsureComponent<WaterParkCreature>();

                    //Create an empty WaterParkCreatureData for us to populate, if it's empty
                    if (wpc.data == null)
                    {
                        //Because I'm setting this in LiveMixin.Awake, this SHOULD NOT trigger
                        logger.LogError($"Error! WaterParkCreature component data for {techType} is null!");
                    }

                    //Use appropriate size to calculate WPC data
                    SetWaterParkData(ref wpc.data, size, techType);

                    return true;
                }
            }
            else
            {
                ErrorMessage.AddError($"{techType} is not in the waterpark reference dictionary!");
            }

            //Return false if any of the if statements are false
            return false;
        }

        public static void SetWaterParkData(ref WaterParkCreatureData data, float modifier, TechType techType)
        {
            //NOTE!! Each particular creature shares a WaterParkCreatureData (e.g. Hoopfish_WaterParkCreatureData)
            //This means two things; one, I can't change one SpineFish without changing the other, and two, I need to create one for the creatures that don't usually go in containment, like leviathans

            //2nd NOTE!! When loading into a save, with a creature such as a reaper in containment (a creature I added a waterparkcomponent to), the data is empty and null upon loading the save
            //Will likely need to look into repopulating the data every time I load in
            data.initialSize = modifier * 0.1f;
            data.maxSize = modifier * 0.6f;
            data.outsideSize = modifier;

            //Check whether the creature should be pickupable outside containment or not (default value is true, if not in dictionary)
            if(PickupableReference.ContainsKey(techType))
            {
                //Whether the creature can be picked up is equal to whether or not the creature's size is within the set range
                var (min, max) = PickupableReference[techType];
                bool withinRange = (modifier >= min && modifier <= max);
                data.isPickupableOutside = withinRange;
            }

            //If creature is large, it cannot breed in containment
            data.canBreed = GetSizeClass(techType) != SizeClass.Large;
        }

        public static bool GetInsideWaterPark(GameObject creature)
        {
            //DEBUG!!
            TechType techType = CraftData.GetTechType(creature);

            if(creature.GetComponentInParent<WaterPark>() != null)
            {
                //logger.LogInfo($"{techType} is in a WaterPark");
                return true;
            }

            //logger.LogInfo($"{techType} is not in a WaterPark");
            return false;
        }

        //Just an easier way to read the sizeClass of the creatures
        public enum SizeClass { None, Small, Medium, Large }

        private static SizeClass GetSizeClass(TechType techType)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < CreatureSizeClassReference[i].Length; j++)
                {
                    if (CreatureSizeClassReference[i][j] == techType)
                    {
                        //We return i (+1 as None is 0) as the size class, as it refers to which of the 3 size arrays we found the TechType match in
                        //We use the SizeClass array in future references, for legibility
                        SizeClass sizeClass = (SizeClass)(i + 1); //This turns the int result into its appropriate enum counterpart; e.g. 1 becomes SizeClass.Small
                        return sizeClass; //No need to keep going through the loop, if we've found our techtype already
                    }
                }
            }

            //If no matches, size class equals none
            return SizeClass.None;
        }

        private static float GetCreatureSizeModifier(TechType techType)
        {
            //Return a default modifier of 1 if no size class can be found in the reference array
            var modifier = 1.0f;

            if (!config.ComplexSizeEnabled)
            {
                #region Size Class Modifier
                //Try to get the size class of the given TechType
                SizeClass sizeClass = GetSizeClass(techType);

                //If we found a size class, generate a modifier for it, then break from the loop
                if (sizeClass != SizeClass.None)
                {
                    switch (sizeClass)
                    {
                        case SizeClass.Small:
                            modifier = GenerateSizeModifier(config.SmallCreatureMinSize, config.SmallCreatureMaxSize);
                            break;
                        case SizeClass.Medium:
                            modifier = GenerateSizeModifier(config.MedCreatureMinSize, config.MedCreatureMaxSize);
                            break;
                        case SizeClass.Large:
                            modifier = GenerateSizeModifier(config.LargeCreatureMinSize, config.LargeCreatureMaxSize);
                            break;
                    }
                }
                else
                {
                    logger.LogWarning($"Error! Could not retrieve size class for TechType {techType}!");
                }
                #endregion
            }
            else
            {
                #region Complex Size Modifier
                //Try to get the size range of the given TechType
                if (config.CreatureSizeRangeReference.ContainsKey(techType))
                {
                    //Generate a size modifier based on the min and max values of the dictionary in config (text file able to be manually edited by users)
                    var (min, max) = config.CreatureSizeRangeReference[techType];
                    modifier = GenerateSizeModifier(min, max);
                }
                #endregion
            }

            return modifier;
        }

        private static float GenerateSizeModifier(float minSize, float maxSize)
        {
            //Return a random size between the min and max
            System.Random rand = new System.Random();

            //Return an int value, between min and max values, both multiplied by 10
            //NOTE!! We multiply both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            var modifierByTen = rand.Next((int)(minSize * 10), (int)(maxSize * 10));

            //Divide the modifier back down by 10, meaning we have a modifier to 1 decimal place.
            var modifier = (float)modifierByTen / 10;

            return modifier;
        }

        private static void SetSize(GameObject creature, float modifier)
        {
            creature.transform.localScale = new Vector3(modifier, modifier, modifier);
        }

        private static float GetSize(GameObject creature)
        {
            //NOTE!! Be wary this won't differentiate if a creature is in containment or not, as it will be smaller there than out in the ocean
            var size = creature.transform.localScale.x;

            return size;
        }
    }
}
