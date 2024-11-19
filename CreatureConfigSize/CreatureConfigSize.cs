using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using static CreatureConfigSize.References;
using UnityEngine;
using Nautilus.Handlers;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace CreatureConfigSize
{
    [HarmonyPatch]
    internal class CreatureConfigSize
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        [HarmonyPrefix]
        public static void PrePlayerStart()
        {
            SetCreatureInvInfo();

            /*Sprites needed
             * Reaper (Done)
             * Sea Dragon (Done)
             * Ghost Leviathan (same can be used for both adult and juvenile) (Done)
             * Reefback baby (can hopefully retrieve the one used for adult to use for baby as well)
             * Sea Emperor Juvenile (Done)
             * Sea Emporer Baby (Done)
             * Sea Treader (Done)
             * Skyray (Done)
             * Cave Crawler (Done)
             * Blood Crawler (Done)
             * Shuttlebug (Not sure if they don't already have an icon??? CHECK!!) (Done)
             * Warper (Done)
             * Lava Larva (Done)
             * Red Ghostray (Done)
             * Blue Ghostray (Done)
             * Spineel (Done)
             * Bleeder (Done)
             * Biter (Done)
             * Blighter (Done)
            */
            
            //DEBUG!! Showcase what options are on or off
            logger.LogInfo($"All Pickupable = {config.AllowAllPickupable}");
            logger.LogInfo($"All WaterPark = {config.AllowAllWaterPark}");
        }

        internal class CreatureInvInfo
        {
            internal TechType techType { get; set; } //The TechType of the creature
            internal int invSize; //How big the creature is in the inventory (e.g. 4 is 4*4 large in inventory)
            internal string iconName; //The filename of the icon file
            internal string name; //The displayed name of the creature in the inventory
            internal string tooltip; //The displayed tooltip of the creature in the inventory

            //Create a constructor to streamline defining entries
            internal CreatureInvInfo(TechType techType, int invSize, string iconName, string name, string tooltip)
            {
                this.techType = techType;
                this.invSize = invSize;
                this.iconName = iconName;
                this.name = name;
                this.tooltip = tooltip;
            }

            //Create a deconstructor to streamline retrieving entries
            public void Deconstruct(out TechType techType, out int invSize, out string iconName, out string name, out string tooltip)
            {
                techType = this.techType;
                invSize = this.invSize;
                iconName = this.iconName;
                name = this.name;
                tooltip = this.tooltip;
            }
        }

        internal static List<CreatureInvInfo> CreatureInvList = new List<CreatureInvInfo>()
        {
            new CreatureInvInfo(TechType.Biter, 1, "biter_icon", "Biter", "Small, aggressive carnivore."),
            new CreatureInvInfo(TechType.Bleeder, 1, "bleeder_icon", "Bleeder", "Parasite attracted to blood."),
            new CreatureInvInfo(TechType.ReaperLeviathan, 4, "reaper_icon", "Reaper Leviathan", "Vast leviathan with aggressive tendencies."),
            new CreatureInvInfo(TechType.SeaEmperorJuvenile, 4, "sea_emperor_icon", "Sea Emperor Leviathan Juvenile", "Vast leviathan capable of producing Enzyme 42."),
            new CreatureInvInfo(TechType.SeaEmperorBaby, 3, "sea_emperor_baby_icon", "Sea Emperor Leviathan Baby", "Juvenile leviathan capable of producing Enzyme 42. Taken shortly after being born.")
        };

        internal static void SetCreatureInvInfo()
        {
            for(int i = 0; i < CreatureInvList.Count; i++)
            {
                var (techType, invSize, iconName, name, tooltip) = CreatureInvList[i];
                //Set size of creature in inventory (invSize * invSize)
                //TODO!! Check if the EntitySlot.Types are relevant when deciding what size a creature is in inventory
                CraftData.itemSizes.Add(techType, new Vector2int(invSize, invSize));

                //Register the sprite to the desired TechType
                //Sprite reaperIcon = SpriteHandler.RegisterSprite(SpriteManager.Group.Item, "reaperIcon", "filepath");
                //NOTE!! Regarding the reaper sprite, I scaled in down to 128*128, then sharpened by 1

                //Get the filepath to the mod assets folder
                string iconFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
                //Apply icon sprite to the desired techtype
                SpriteHandler.RegisterSprite(techType, Path.Combine(iconFilePath, iconName + ".png"));
                //Add display name to the desired techtype
                LanguageHandler.SetTechTypeName(techType, name);
                //Add item description to the desired techtype
                LanguageHandler.SetTechTypeTooltip(techType, tooltip);
            }
        }

        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Awake))]
        [HarmonyPostfix] //Using LiveMixin because creatures in containment don't trigger Creature events (because their creature component is disabled)
        //Using .Awake to set up the placeholder WPC data, as otherwise by .Start it's pulling null references and breaking
        public static void PostLiveMixinAwake(Creature __instance)
        {
            GameObject creature = __instance.gameObject;
            TechType techType = CraftData.GetTechType(creature);
            //logger.LogInfo($"(PostLiveMixinAwake) entity is {techType}");

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
            //Reference the gameObject Class directly, as we don't need the Creature class specifically
            GameObject creature = __instance.gameObject;

            //Retrieve the TechType of the creature; we'll use this for filtering from the reference tables for only things we intend to resize
            TechType techType = CraftData.GetTechType(creature);

            //If creature techtype is part of our reference tables, i.e. is this a creature we're looking to randomise (means we won't touch modded creatures accidentally)
            if (PickupableReference.ContainsKey(techType))
            {
                logger.LogInfo($"Creature {techType} found");
                ErrorMessage.AddMessage($"Creature {techType} found");

                //As Reefbacks share the same TechType with baby Reefbacks, despite both existing, manually apply it, so they can be seperately resized
                //NOTE!! Below is the Class ID for the ReefbackBaby prefab, so this checks if it's a reefback baby
                if (creature.GetComponent<PrefabIdentifier>().ClassId == "34765384-821f-41ad-b716-1b68c507e4f2")
                {
                    //DEBUG!!
                    logger.LogInfo($"{creature.GetComponent<PrefabIdentifier>().ClassId}");

                    creature.GetComponent<TechTag>().type = TechType.ReefbackBaby;
                    //Update the local variable for techType, so that the TechType is up to date for this method going forward
                    techType = TechType.ReefbackBaby;
                }

                //Retrieve the unique id of this creature
                //NOTE!! Remember, "id" is the private value of UniqueIdentifier, "Id" is the exposed, public accessor; we need to use "Id"
                string creatureId = creature.GetComponent<PrefabIdentifier>().Id;
                logger.LogInfo($"ID of {techType} = {creatureId}");

                //Check whether we've already randomised the size of this creature, as its id should already be in our dictionary if so
                if (!creatureSizeInfoList.creatureSizeDictionary.ContainsKey(creatureId))
                {
                    logger.LogInfo($"Size not randomised/ID not logged.");

                    //Generate a modifier for the creature's size
                    //Based on either the creature's size class, retrieved from the CreatureSizeReference array, or the min and max values in the json file, if complex is on
                    float modifier = GetCreatureSizeModifier(techType);
                    logger.LogInfo($"Creature Modifier = {modifier}");

                    //Once we've retrieved the modifier, apply it by multiplying the creature's size by the modifier
                    SetSize(creature, modifier);
                    ErrorMessage.AddMessage($"Changed size of {techType} to {modifier}");

                    //Add unique id and applied size to dictionary, to document that we *have* randomised this creature's size
                    creatureSizeInfoList.creatureSizeDictionary.Add(creature.GetComponent<PrefabIdentifier>().Id, modifier);
                }
                else
                {
                    logger.LogInfo($"Size already randomised/ID already logged.");

                    float modifier = creatureSizeInfoList.creatureSizeDictionary[creatureId];

                    //Reapply the modifier, in case it expires if we reload with the creature unloaded
                    SetSize(creature, modifier);
                    ErrorMessage.AddMessage($"Changed size of {techType} to {modifier}");
                }

                //logger.LogInfo($"Length of ID Dictionary = {creatureSizeInfoList.creatureSizeDictionary.Count}");

                var size = GetSize(creature);
                logger.LogInfo($"Creature Size After = {size}");

                //Scale floater's bouyancy with their new size, because why not; it's cool!
                if(techType == TechType.Floater) { creature.GetComponent<Floater>().buoyantForce = 8 * size; }

                //Need to check whether creature can be picked up and placed in alien containment, regardless of whether the creature has had its size randomised or not, as these reset at startup
                //Check whether the creature is eligible to be picked up (and have the Pickupable component) or not
                CheckPickupableComponent(creature, size);

                //Check whether the creature is eligible to be placed in alien containment up (and have the WPC component) or not
                CheckWaterParkCreatureComponent(creature, size);
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
                    logger.LogInfo($"(Pickupable) {techType} is already inside alien containment! Calcuating original size modifier from current size!");

                    //By performing the calculation to get maxSize for WPC data (x * 0.6), but in reverse (x / 0.6), we get our old size back and original size modifier
                    modifier = size / 0.6f;

                }
                else
                {
                    logger.LogInfo($"(Pickupable) {techType} is not inside alien containment! Using current size for size modifier!");

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
                    if(componentExists || !insideWaterPark)
                    {
                        var component = creature.GetComponent<Pickupable>();
                        UnityEngine.Object.Destroy(component);
                    }
                }

                //If *somehow* the creature is in alien containment *without* the pickupable component, add it in, as a final check
                if(insideWaterPark && !componentExists)
                {
                    logger.LogWarning($"Warning! {techType} in alien containment without pickupable component! Adding component.");
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

        internal static void SetWaterParkData(ref WaterParkCreatureData data, float modifier, TechType techType)
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

        internal static bool GetInsideWaterPark(GameObject creature)
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
                    logger.LogError($"Error! Could not retrieve size class for TechType {techType}!");
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

        internal static void SetSize(GameObject creature, float modifier)
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
