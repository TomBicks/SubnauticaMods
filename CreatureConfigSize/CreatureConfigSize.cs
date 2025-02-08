using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using static CreatureConfigSize.References;
using UnityEngine;
using Nautilus.Handlers;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UWE;
using Nautilus.Extensions;

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

            //DEBUG!! TEST TO SPAWN A SANDSHARK USING CLASSID
            //2nd method seems to work
            //AssetReferenceGameObject sandshark = new AssetReferenceGameObject("5e5f00b4-1531-45c0-8aca-84cbd3b580a4");
            //sandshark.InstantiateAsync();
            AssetReferenceGameObject sandshark2 = new AssetReferenceGameObject("WorldEntities/Creatures/SandShark.prefab");
            sandshark2.InstantiateAsync();

            //DEBUG!! Showcase what options are on or off
            logger.LogInfo($"All Pickupable = {config.AllowAllPickupable}");
            logger.LogInfo($"All WaterPark = {config.AllowAllWaterPark}");
            logger.LogInfo($"Complex Size Calculations = {config.ComplexSizeEnabled}");
        }

        //Create list of creatures and their info to add to the game
        internal static List<CreatureInvInfo> CreatureInvList = new List<CreatureInvInfo>()
        {
            //TODO!! Should I see if I can make some of the bigger creatures in inventory's sprites bigger? Like ghost and sea dragon having a bigger sprite, so it's less blurry in inventory?
            new CreatureInvInfo(TechType.Biter, 1, "biter_icon", "Biter", "Small, aggressive carnivore."),
            new CreatureInvInfo(TechType.Bleeder, 1, "bleeder_icon", "Bleeder", "Small parasite attracted to blood."),
            new CreatureInvInfo(TechType.Blighter, 1, "blighter_icon", "Blighter", "Small, aggressive carnivore. Largely blind."),
            new CreatureInvInfo(TechType.Shuttlebug, 2, "blood_crawler_icon", "Blood Crawler", "Small, aggressive scavengers with elongated legs. Semi-terrestrial, though largely found in the deep sea."),
            new CreatureInvInfo(TechType.CaveCrawler, 2, "cave_crawler_icon", "Cave Crawler", "Small, aggressive scavengers. Semi-terrestrial."),
            new CreatureInvInfo(TechType.GhostRayRed, 3, "crimson_ray_icon", "Crimson Ray", "Docile, deep-sea ray capable of surviving extreme temperatures."),
            new CreatureInvInfo(TechType.GhostLeviathan, 5, "ghost_icon", "Ghost Leviathan", "Vast leviathan with aggressive tendencies. Hunts until it grows so large that it must migrate beyond the Crater's Edge to sustain itself."),
            new CreatureInvInfo(TechType.GhostLeviathanJuvenile, 4, "ghost_icon", "Ghost Leviathan Juvenile", "Vast leviathan with aggressive tendencies. Hunts in the Lost River, before migrating to the surface as it reaches adulthood."),
            new CreatureInvInfo(TechType.GhostRayBlue, 3, "ghostray_icon", "Ghostray", "Docile, deep-sea ray with entirely translucent skin."),
            new CreatureInvInfo(TechType.LavaLarva, 2, "lava_larva_icon", "Lava Larva", "Parasite that can drain nearby thermal and electric energy."),
            new CreatureInvInfo(TechType.ReaperLeviathan, 4, "reaper_icon", "Reaper Leviathan", "Vast leviathan with aggressive tendencies. Uses echo-location to hunt its prey."),
            new CreatureInvInfo(TechType.SpineEel, 3, "river_prowler_icon", "River Prowler", "Large, aggressive predator. Capable of pulling in prey with its tendrils."),
            new CreatureInvInfo(TechType.SeaDragon, 5, "sea_dragon_icon", "Sea Dragon Leviathan", "Vast leviathan with aggressive tendencies. Capable of consuming and expelling molten materials."),
            new CreatureInvInfo(TechType.SeaEmperorJuvenile, 4, "sea_emperor_icon", "Sea Emperor Leviathan Juvenile", "Vast leviathan capable of producing Enzyme 42. Even at this size, this is only a juvenile."),
            new CreatureInvInfo(TechType.SeaEmperorBaby, 3, "sea_emperor_baby_icon", "Sea Emperor Leviathan Baby", "Juvenile leviathan capable of producing Enzyme 42. Taken shortly after being born."),
            new CreatureInvInfo(TechType.SeaTreader, 4, "sea_treader_icon", "Sea Treader Leviathan", "Large, herbivorous leviathan. Migratory."),
            new CreatureInvInfo(TechType.Skyray, 2, "skyray_icon", "Skyray", "Small, avian often found over large landmasses. Cannot swim."),
            new CreatureInvInfo(TechType.Warper, 3, "warper_icon", "Warper", "Aggressive creature capable of teleportation.") //Make it have two options? Nah, even the original entry doesn't have two versions, just two seperate entries.
        };

        //Iterate through each of the creatures and add their info
        internal static void SetCreatureInvInfo()
        {
            for(int i = 0; i < CreatureInvList.Count; i++)
            {
                var (techType, invSize, iconName, name, tooltip) = CreatureInvList[i];
                //Set size of creature in inventory (invSize * invSize)
                //TODO!! Check if the EntitySlot.Types are relevant when deciding what size a creature is in inventory
                CraftData.itemSizes.Add(techType, new Vector2int(invSize, invSize));

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

        [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.OnAddToWP))]
        [HarmonyPostfix] //After adding to containment, check if creature needs any additional changes (usually reenabling certain components, so they look nicer in containment
        public static void PostOnAddToWP(WaterParkCreature __instance)
        {
            GameObject creature = __instance.gameObject;

            TechType techType = CraftData.GetTechType(creature);

            Creature component = null;

            switch (techType)
            {
                case TechType.CaveCrawler:
                    logger.LogInfo($"Enabling {techType}");
                    ErrorMessage.AddMessage($"Enabling {techType}");
                    component = creature.GetComponent<Creature>();
                    break;
                case TechType.Shuttlebug: //Blood Crawler
                    logger.LogInfo($"Enabling {techType}");
                    ErrorMessage.AddMessage($"Enabling {techType}");
                    component = creature.GetComponent<Creature>();
                    break;
                case TechType.SeaDragon:
                    logger.LogInfo($"Enabling {techType}");
                    ErrorMessage.AddMessage($"Enabling {techType}");
                    component = creature.GetComponent<Creature>();
                    break;
                case TechType.Skyray:
                    logger.LogInfo($"Enabling {techType}");
                    ErrorMessage.AddMessage($"Enabling {techType}");
                    Skyray birdComponent = creature.GetComponent<Skyray>();
                    birdComponent.drowning = true; //Makes sure the skyray is animating that it is drowning in containment
                    component = birdComponent;
                    break;
                case TechType.Warper:
                    logger.LogInfo($"Warper is warping out of containment");
                    ErrorMessage.AddMessage($"Warper has warped out of containment!");
                    System.Random rand = new System.Random();
                    if(rand.Next(1, 20) == 20) //1/20 chance for it to warp out of containment (this also occurs on load-in)
                    {
                        Warper warperComponent = creature.GetComponent<Warper>();
                        warperComponent.WarpOut();
                    }
                    break;
            }

            if(component != null)
            {
                component.enabled = true;
            }
            else
            {
                //No error for Warper, as it's unique in that it's not reenabling a component, but just having a chance to warp out of containment
                if(techType != TechType.Warper)
                {
                    logger.LogInfo($"Error! No Creature component found for {techType}!");
                    ErrorMessage.AddMessage($"Error! No Creature component found for {techType}!");
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
                    float modifier = GetCreatureSizeModifier(creature);
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
                    logger.LogInfo($"Changed size of {techType} to {modifier}");
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
            //Result of whether the creature needs a Pickupable component or not
            bool result = false;

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

                //Use original size modifier to determine if eligible for pickupable component (also eligibile if set to allow all or if *already in* containment)
                if ((modifier >= min && modifier <= max) || config.AllowAllPickupable ||insideWaterPark)
                {
                    //If creature is eligible for the component and doesn't have one, add it; return true if it needed one at all
                    if(!componentExists)
                    {
                        creature.AddComponent<Pickupable>();
                    }
                    result = true;
                }
                else
                {
                    //If creature is ineligable for the component and has one, remove it (and will return false by default)
                    //NOTE!! If creature is in alien containment, we DO NOT remove this component; IsPickupableOutside is used instead to remove the component when the creature is placed outside
                    if(componentExists && !insideWaterPark)
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

            //Return whether the creature needs a Pickupable component or not
            return result;
        }

        //TODO!! Look into hooking into when a creatures hatches from an egg, and set its size there randomly (can be outside the range of pickupable, but within the range of what will fit
        //in alien containment; this is how you can get bigger sizes in there, if you can't pick them up); they'll still be about 33% of their size in the tank though, so they grow as
        //newly hatched fish do

        //Check whether the creature's size makes it eligible or not for the WaterParkCreature component, and to add it or remove it
        public static bool CheckWaterParkCreatureComponent(GameObject creature, float size)
        {
            //Result of whether the creature needs a WaterParkCreature component or not
            bool result = false;

            //Generate techtype to check the dictionary for creature's entry
            TechType techType = CraftData.GetTechType(creature);

            //Whether the creature has a WaterParkCreature component already or not
            bool componentExists = !(creature.GetComponent<WaterParkCreature>() == null);

            //Whether the creature is in alien containment or not
            bool insideWaterPark = GetInsideWaterPark(creature);

            if (WaterParkReference.ContainsKey(techType))
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

                //DEBUG!!
                logger.LogWarning($"(WaterParkCreature) {techType}'s modifier is {modifier}");

                //Use original size modifier to determine if eligible for WPC component (also eligibile if set to allow all or if *already in* containment)
                if ((modifier >= min && modifier <= max) || config.AllowAllWaterPark || insideWaterPark)
                {
                    WaterParkCreature wpc = creature.EnsureComponent<WaterParkCreature>();

                    //If creature is eligible for the component and doesn't have one, add it; return true if it needed one at all
                    if (!componentExists)
                    {
                        //Ensure creature has WaterParkCreature component if it needs one added, and make sure it has blank data
                        //If we need to create a component, that means the previous reference to 'wpc' was null, so we override it with this new and shiny one
                        wpc = creature.EnsureComponent<WaterParkCreature>();
                        wpc.data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
                    }

                    result = true;
                }
                else
                {
                    //If creature is ineligable for the component and has one, remove it (and will return false by default)
                    //NOTE!! If creature is in alien containment, we DO NOT remove this component
                    if(componentExists && !insideWaterPark)
                    {
                        var component = creature.GetComponent<WaterParkCreature>();
                        UnityEngine.Object.Destroy(component);
                    }
                }

                //Use appropriate size to calculate WPC data
                //NOTE!! Always update WaterParkCreature data, so long as the component exists, just in case it's a creature in containment that shouldn't be there
                if(!(creature.GetComponent<WaterParkCreature>() == null))
                {
                    SetWaterParkData(ref creature.GetComponent<WaterParkCreature>().data, modifier, techType);
                }
            }
            else
            {
                logger.LogError($"Error! {techType} is not in the waterpark reference dictionary!");
            }

            //Return whether the creature needs a WaterParkCreature component or not
            return result;
        }

        internal static void SetWaterParkData(ref WaterParkCreatureData data, float modifier, TechType techType)
        {
            //NOTE!! WPC data needs to be repopulated every new session for creatures with usually no WPC component, and for every other creature we want to differentiate
            logger.LogWarning($"(WaterParkData) Setting {techType}'s WPC data values to initialSize = {modifier * 0.1f}, maxSize = {modifier * 0.6f}, and outsideSize = {modifier}");
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

            //TRY AND GET GUID OF PEEPER; WE KNOW IT SHOULD BE 41070d44a15a89941ba4c2a459370e98
            if (techType == TechType.ReaperLeviathan)
            {
                //AssetReferenceGameObject child = AssetReference; //???
                //Need to retrive GUID for the prefabs of the creatures I'm gonna be making 'libe-birth'
                logger.LogInfo($"Peeper added to WP");
                ErrorMessage.AddMessage($"Peeper added to WP");

                //NOTE!! Testing on this shows that .prefab results can *not* create peepers to be born
                //However, using the GUID I found and using that to generate an AssetReference does work
                //Method 1 - returns .prefab
                string test;
                PrefabDatabase.TryGetPrefabFilename("3fcd548b-781f-46ba-b076-7412608deeef", out test);
                AssetReferenceGameObject testPeeper = new AssetReferenceGameObject(test);
                logger.LogInfo($"Test GUID = {test}");

                //Method 2 - returns .prefab (RuntimeKey is valid)
                AssetReferenceGameObject simplePeeper = new AssetReferenceGameObject("3fcd548b-781f-46ba-b076-7412608deeef");
                logger.LogInfo($"Test Peeper GUID = {simplePeeper}");
                logger.LogInfo($"Test Peeper Runtime Key Validity = {simplePeeper.RuntimeKeyIsValid()}");

                //ERROR!! UnityEditor.AssetDatabase only works within the editor, as it's a part of UnityEditor
                //string GUID = AssetDatabase.AssetPathToGUID("asset path");

                //Method 3 - returns GUID (RuntimeKey is valid)
                AssetReferenceGameObject peeper = new AssetReferenceGameObject("41070d44a15a89941ba4c2a459370e98");
                logger.LogInfo($"Peeper = {peeper}");
                logger.LogInfo($"Peeper Runtime Key Validity = {peeper.RuntimeKeyIsValid()}");
                logger.LogInfo($"Peeper GUID = {peeper.AssetGUID}");

                //Test 4 - resource comes up blank
                GameObject resource = (GameObject)Resources.Load("Assets/AddressableResources/WorldEntities/Creatures/Peeper.prefab");
                //AssetReferenceGameObject resourcePeeper = AssetReferenceGameObject(resource);
                logger.LogInfo($"Resource Peeper = {resource}");
                //logger.LogInfo($"Resource Peeper GUID = {resource}");

                //Method 5 - WORKS! (.ForceValid() is key, no pun intended)
                AssetReferenceGameObject sandshark = new AssetReferenceGameObject("WorldEntities/Creatures/SandShark.prefab");
                logger.LogInfo($"Sandshark = {sandshark}");
                //sandshark.RuntimeKey = ("WorldEntities/Creatures/SandShark.prefab");
                sandshark.ForceValid();
                logger.LogInfo($"Sandshark Runtime Key Validity = {sandshark.RuntimeKeyIsValid()}");
                logger.LogInfo($"Sandshark GUID = {sandshark.AssetGUID}");

                data.canBreed = true;
                data.eggOrChildPrefab = sandshark;
            }
            /*else if (techType == TechType.ReaperLeviathan)
            {
                data.canBreed = true;
                AssetReferenceGameObject peeper = new AssetReferenceGameObject("41070d44a15a89941ba4c2a459370e98");
                data.eggOrChildPrefab = peeper;
            }*/
            else
            {
                //If creature is large, it cannot breed in containment
                data.canBreed = GetSizeClass(techType) != SizeClass.Large;
            }
        }

        /*[HarmonyPatch(typeof(AddressablesImpl), nameof(AddressablesImpl.EvaluateKey))]
        [HarmonyPostfix] //After adding to containment, check if creature needs any additional changes (usually reenabling certain components, so they look nicer in containment
        public static void PostEvaluateKey(Object __instance)
        {

        }*/

        internal static bool GetInsideWaterPark(GameObject creature)
        {
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

        private static float GetCreatureSizeModifier(GameObject creature)
        {
            //Declare min and max variables
            var (min, max) = (1.0f,1.0f);

            //Generate techtype to check the dictionary for creature's entry
            TechType techType = CraftData.GetTechType(creature);

            //Whether the creature is in alien containment or not
            bool insideWaterPark = GetInsideWaterPark(creature);

            if (!config.ComplexSizeEnabled)
            {
                #region Size Class Modifier
                //Try to get the size class of the given TechType
                SizeClass sizeClass = GetSizeClass(techType);

                //If we found a size class, retrieve minimum and maximum size ranges
                if (sizeClass != SizeClass.None)
                {
                    switch (sizeClass)
                    {
                        case SizeClass.Small:
                            min = config.SmallCreatureMinSize;
                            max = config.SmallCreatureMaxSize;
                            break;
                        case SizeClass.Medium:
                            min = config.MedCreatureMinSize;
                            max = config.MedCreatureMaxSize;
                            break;
                        case SizeClass.Large:
                            min = config.LargeCreatureMinSize;
                            max = config.LargeCreatureMaxSize;
                            break;
                    }
                }
                else
                {
                    logger.LogError($"Error! Could not retrieve size class ranges for TechType {techType}!");
                }
                #endregion
            }
            else
            {
                #region Complex Size Modifier
                //Try to get the size range of the given TechType
                if (config.CreatureSizeRangeReference.ContainsKey(techType))
                {
                    //Retrieve minimum and maximum size ranges from the dictionary in config (text file able to be manually edited by users)
                    (min, max) = config.CreatureSizeRangeReference[techType];
                }
                else
                {
                    ErrorMessage.AddError($"Error! {techType} is not in the size range reference dictionary!");
                }
                #endregion
            }

            //If the creature is in alien containment, then the max value must not exceed the containment's limits, so we change it to make sure
            if (insideWaterPark && WaterParkReference.ContainsKey(techType)) 
            {
                logger.LogWarning($"Warning! {techType}'s maximum size range of {max} exceeds WaterPark safe capacity! Clamping to {WaterParkReference[techType].max}.");
                max = WaterParkReference[techType].max; 
            }

            //If minimum size range is greater than maximum, make them equal to the current max size
            if (min > max)
            {
                logger.LogError($"Error! Minimum size for {techType} of {min} is greater than maximum size of {max}! Raising minimum to match maximum.");
                min = max;
            }

            //Returns a default modifier of 1 if no size class can be found in the reference array, as both min and max will both still be 1
            var modifier = GenerateSizeModifier(min, max);

            //If the creature's size modifier is being determined whilst in alien containment, such as with a hatching creature, make sure it's clear it's 60% smaller in containment than outside
            if (insideWaterPark) { modifier *= 0.6f; }

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
