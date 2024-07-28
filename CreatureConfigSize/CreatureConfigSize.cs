using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using static CreatureConfigSize.References;
using UnityEngine;
using System.Collections.Generic;

namespace CreatureConfigSize
{
    [HarmonyPatch]
    internal class CreatureConfigSize
    {
        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPostfix] //Postfix means less chance of missing setting any creatures' size
        public static void PostCreatureStart(Creature __instance)
        {
            //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
            GameObject creature = __instance.gameObject;

            TechType techType = CraftData.GetTechType(creature);

            if (techType != TechType.None)
            {
                //logger.LogDebug($"Setting size of TechType {techType}");
                //ErrorMessage.AddDebug($"Setting size of TechType {techType}");

                //Generate a modifier based on the creature's size class, retrieved from the CreatureSizeReference array
                float modifier = GetCreatureSizeModifier(techType); ;

                //Once we've retrieved the modifier, apply the change to size, by the modifier
                //NOTE!! I wonder if there's a way to save the new size between loading and saving? It's clear they still randomise when loading it, from Start...unless I'm doing that?
                //Is there a way to check if I've already randomised this creature's size?
                //YES!! I AM THE ONE CHANGING IT!! So I'm re-randomising every time, but the size is totally saved. So if I check if it's not size 1.0 local scale, than it'll work!
                if (GetSize(creature) == 1)
                {
                    ChangeSize(creature, modifier);
                    ErrorMessage.AddMessage("Changing Size");

                    ErrorMessage.AddMessage($"Size of {techType} = {GetSize(creature)}");

                    //NOTE!! If i use the reference array above, this switch statement would *only* be for unique changes made!
                    //NOTE!! Can also use this for any changes unique to the creature we need to make, if any; probably jsut for leviathans
                    switch (techType)
                    {
                        case TechType.ReaperLeviathan:
                            #region Notes about creature properties
                            //NOTE!! If I want to deal with velocity, will have to deal with leash movement of leviathans too
                            //NOTE!! Perhaps I can increase the speed of the leviathan, whilst inversely decreasing the turning speed of it, as it gets larger?
                            //Probably no need to increase the turning speed of smaller leviathans; they seem to get buggy when it's put above 3
                            //If they're faster larger, do I need to increase their leash distance a bit? Is that dangerous?

                            //Regarding pitch, which can be set, 0.8 is the deepest without being ridiculous I think, same for 1.4 or 1.3 for highest
                            //Volume can be changed too
                            //Only issue with pitch is the audio is quicker or slower based on it, so it can loop back on itself by the looks
                            //Under FMOD CustomLoopingEventWithCallback, triggering on the Roar animation
                            //Then the eventInstance 'evt' was where I changed the sound parameters

                            //Seems there are two values to determine for movement speed; moveForward and velocity
                            //it seems velocity caps how fast the creature can move, whilst forward is perhaps the forward momentum it can generate?
                            //So it keeps moving forward, up to the velocity limit?

                            //Some example code for how it might work altering a creature based on its modifier
                            //creature.gameObject.GetComponent<FMOD_CustomEmitter>().evt.setParameterValue("volume", 2.5f);

                            //REGARDING SIZE - 0.1 or 0.2 for a reaper, making them 0.06 or 0.12 in scale in the alien containment, is the max size, anything above that will not fit, and thus cannot be picked up
                            #endregion
                            if(modifier <= 0.2)
                            {
                                creature.AddComponent<Pickupable>();
                            }
                            break;
                        default:
                            break;
                    }

                    //Check via reference whether the creature is eligible to be picked up (and have the Pickupable component)
                    if(PickupableReference.ContainsKey(techType))
                    {
                        ErrorMessage.AddMessage($"{techType} is using pickupableReference");

                        //Size range within which a creature is made able to be picked up
                        //Being outside of this range means the creature will have the Pickupable component removed, if it has one
                        var (min, max) = PickupableReference[techType];

                        //Whether the creature has a Pickupable component already or not
                        bool componentExists = !(creature.GetComponent<Pickupable>() == null);

                        if (modifier >= min && modifier <= max)
                        {
                            //If creature is eligible for the component and doesn't have one, add it
                            if (!componentExists)
                            {
                                creature.AddComponent<Pickupable>();
                            }
                        }
                        else
                        {
                            //If creature is ineligable for the component and has one, remove it
                            if(componentExists)
                            {
                                var component = creature.GetComponent<Pickupable>();
                                Object.Destroy(component);
                            }
                        }
                    }
                    //DEBUG!! Temporary code until I add in the reference for every creature
                    else
                    {
                        ErrorMessage.AddError($"{techType} is not in the pickupable reference dictionary!");

                        //If suitable size for alien containment, make pickupable and update WaterParkCreature component
                        if (creature.GetComponent<Pickupable>() == null)
                        {
                            creature.AddComponent<Pickupable>();
                        }
                    }

                    //NOTE!! USE REF LIKE YOU DID WITH DAMAGE! IT HELPS FIX THE PRIVACY ISSUE
                    //https://www.geeksforgeeks.org/ref-in-c-sharp/
                    //NOTE!! Creatures in containment will not trigger Creature.Start when loading in; they will only when released from the inventory
                    if (creature.GetComponent<WaterParkCreature>() != null)
                    {
                        //According to Indigo, I might be able to duplicate the data, change the values, and assign it back to the changed fish; worth a shot
                        WaterParkCreatureData newData = ScriptableObject.CreateInstance<WaterParkCreatureData>();
                        //newData = creature.GetComponent<WaterParkCreature>().data;

                        SetWaterParkData(ref newData, GetSize(creature));

                        //newData.name = creature.GetComponent<WaterParkCreature>().data.name;
                        //newData.initialSize = GetSize(creature);
                        //newData.maxSize = GetSize(creature);

                        creature.GetComponent<WaterParkCreature>().data = newData;
                    }
                    else
                    {
                        creature.AddComponent<WaterParkCreature>();
                        WaterParkCreatureData newData = ScriptableObject.CreateInstance<WaterParkCreatureData>();
                        SetWaterParkData(ref newData, GetSize(creature));
                        creature.GetComponent<WaterParkCreature>().data = newData;
                    }
                }

                //Using this to repopulate the missing WaterParkCreatureData on each reload
                //ERROR!! Creatures in alien containment DO NOT trigger Creature.Start; this WILL NOT work to repopulate the WaterParkCreatureData of creatures already in containment
                /*if (creature.GetComponent<WaterParkCreature>() != null)
                {
                    logger.LogWarning($"{techType} has WaterParkComponent.");

                    WaterParkCreature waterParkComponent = creature.GetComponent<WaterParkCreature>();

                    if (waterParkComponent.data == null)
                    {
                        logger.LogWarning($"{techType} WaterParkComponent data is null! Repopulating!");
                        logger.LogWarning($"Size is {GetSize(creature)}!");

                        WaterParkCreatureData newData = ScriptableObject.CreateInstance<WaterParkCreatureData>();

                        //If creature is inside the alien containment, then it'll be smaller than its max size, and we need to work backwards
                        if (waterParkComponent.isInside)
                        {
                            logger.LogWarning($"{techType} is already inside alien containment! Calcuating original size!");

                            //By performing the calculation to get maxSize (x * 0.6), but in reverse (x / 0.6), we get our old size back
                            var initialSize = GetSize(creature) / 0.6f;
                            SetWaterParkData(ref newData, initialSize);
                        }
                        else
                        {
                            logger.LogWarning($"{techType} is not inside alien containment! Using current size!");

                            //If the creature isn't in alien containment, this means we can just use its current size, as normal, to set the WaterParkCreatureData
                            var currentSize = GetSize(creature);
                            SetWaterParkData(ref newData, currentSize);
                        }

                        //Apply the new WaterParkCreatureData
                        creature.GetComponent<WaterParkCreature>().data = newData;
                        logger.LogWarning($"New WaterParkCreatureData applied!");
                    }
                }*/
            }
            else
            {
                logger.LogWarning($"Error! Creature {__instance.name} has no TechType!");
            }
        }

        [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Start))]
        [HarmonyPostfix] //Postfix means less chance of missing setting any creatures' size
        public static void PostWaterParkStart(WaterParkCreature __instance)
        {
            GameObject creature = __instance.gameObject;
            logger.LogWarning($"(WaterParkCreasture) {__instance.gameObject}");
            TechType techType = CraftData.GetTechType(creature);

            logger.LogWarning($"(WaterParkCreasture) Size of {techType} = {GetSize(creature)}");

            if (__instance.data == null)
            {
                logger.LogWarning($"(WaterParkCreasture) {techType} WaterParkComponent data is null! Repopulating!");
                logger.LogWarning($"(WaterParkCreasture) Size is {GetSize(creature)}!");

                WaterParkCreatureData newData = ScriptableObject.CreateInstance<WaterParkCreatureData>();

                //If creature is inside the alien containment, then it'll be smaller than its max size, and we need to work backwards
                //NOTE!! We specify if currentWaterPark isn't null, as if they are in inventory after being picked up from containment, it'll be a false positive
                if (__instance.isInside && __instance.currentWaterPark != null)
                {
                    logger.LogWarning($"(WaterParkCreasture) {techType} is already inside alien containment! Calcuating original size!");

                    //By performing the calculation to get maxSize (x * 0.6), but in reverse (x / 0.6), we get our old size back
                    var initialSize = GetSize(creature) / 0.6f;
                    SetWaterParkData(ref newData, initialSize);
                }
                else
                {
                    logger.LogWarning($"(WaterParkCreasture) {techType} is not inside alien containment! Using current size!");

                    //If the creature isn't in alien containment, this means we can just use its current size, as normal, to set the WaterParkCreatureData
                    var currentSize = GetSize(creature);
                    SetWaterParkData(ref newData, currentSize);
                }

                //Apply the new WaterParkCreatureData
                __instance.data = newData;
                logger.LogWarning($"(WaterParkCreasture) New WaterParkCreatureData applied!");
            }
        }
        public static void SetWaterParkData(ref WaterParkCreatureData data, float size)
        {
            //ERROR!! Whenever I edit any of these settings for a type of creature, it changes it for all the other instances of this creature!
            //Do they share the same WaterParkCreatureData?
            //NOTE!! Each particular creature shares a WaterParkCreatureData (e.g. Hoopfish_WaterParkCreatureData)
            //This means two things; one, I can't change one SpineFish without changing the other, and two, I need to create one for the creatures that don't usually go in containment, like leviathans

            //2nd ERROR!! When loading into a save, with a creature such as a reaper in containment (a creature I added a waterparkcomponent to), the data is empty and null upon loading the save
            //Will likely need to look into repopulating the data every time I load in
            data.initialSize = size * 0.1f;
            data.maxSize = size * 0.6f;
            data.outsideSize = size;
        }

        //Just an easier way to read the sizeClass of the creatures
        public enum SizeClass { None, Small, Medium, Large }

        private static float GetCreatureSizeModifier(TechType techType)
        {
            //Return a default modifier of 1 if no size class can be found in the reference array
            var modifier = 1.0f;

            if (!config.ComplexSizeEnabled)
            {
                #region Size Class Modifier
                //Return 0 if no size class can be found in the reference array
                var sizeClass = SizeClass.None;

                //Try to get the size class of the given TechType
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < CreatureSizeClassReference[i].Length; j++)
                    {
                        if (CreatureSizeClassReference[i][j] == techType)
                        {
                            //We return i (+1 as None is 0) as the size class, as it refers to which of the 3 size arrays we found the TechType match in
                            //We use the SizeClass array in future references, for legibility
                            sizeClass = (SizeClass)(i + 1); //This turns the int result into its appropriate enum counterpart; e.g. 1 becomes SizeClass.Small
                            break;
                        }
                    }
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
                        break; //No need to keep going through the loop, if we've found our techtype already
                    }
                }

                if (sizeClass == SizeClass.None)
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

        private static void ChangeSize(GameObject creature, float modifier)
        {
            //ErrorMessage.AddMessage($"Multiplying {CraftData.GetTechType(creature)} of size {creature.transform.localScale.x} by modifier {modifier} to size {(modifier)}");
            creature.transform.localScale = new Vector3(modifier, modifier, modifier);
        }

        private static float GetSize(GameObject creature)
        {
            //NOTE!! Should I be wary if creatures are in the alien containment when this is used? Could result int false positives or negatives, as the creature is smaller
            var size = creature.transform.localScale.x;

            return size;
        }
    }
}
