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
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        [HarmonyPrefix]
        public static void PrePlayerStart()
        {
            //Add inventory size of the Reaper Leviathan when picking it up
            CraftData.itemSizes.Add(TechType.ReaperLeviathan, new Vector2int(3, 3));
        }

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
                    ErrorMessage.AddMessage("Changing Size");
                    SetSize(creature, modifier);

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
                            break;
                        default:
                            break;
                    }

                    //Check via reference whether the creature is eligible to be picked up (and have the Pickupable component)
                    CheckPickupableComponent(creature, modifier);
                }
            }
            else
            {
                logger.LogWarning($"Error! Creature {__instance.name} has no TechType!");
            }
        }

        //Create a placeholder WaterParkCreatureData, before the component accesses it, to ensure no null reference errors
        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Awake))]
        [HarmonyPostfix] //By patching LiveMixin, this ensures we can use this for creatures both in and out of containment
        //NOTE!! Originally was patching WaterParkCreature.Start, but as Metious let me know, that is triggered *waaay* too late, and we get null reference errors before then
        public static void CreatePlaceholderWPCData(Creature __instance)
        {
            //logger.LogWarning($"(LiveMixin) {__instance.gameObject}");
            TechType techType = CraftData.GetTechType(__instance.gameObject);

            GetInsideWaterPark(__instance.gameObject);

            //Because many things use LiveMixin, we need to filter; using the WaterParkReference dictionary is perfect here
            //if (WaterParkReference.ContainsKey(techType))
            //{
            //Ensures the component exists; if it doesn't exist, this will create it, meaning no matter what it'll exist from here onwards
            WaterParkCreature wpc = __instance.gameObject.EnsureComponent<WaterParkCreature>();
                logger.LogWarning($"(LiveMixin) Size of {techType} = {GetSize(__instance.gameObject)}");

                if (wpc.data == null)
                {
                    logger.LogWarning($"(LiveMixin) {techType} WaterParkComponent data is null! Creating placeholder!");
                    //ERROR!! So, I'm using LiveMixin, because it means I can assign the data before WaterParkCreature is reference and all the null values start coming in.
                    //But, the issue then is that I need to know whether 'IsInside' is true or not when assigning the WaterParkCreatureData. But the solution to the previous issue
                    //Was to access it at a time when that data is not determined (default false); thus, the creature in containment gets smaller and smaller

                    //SO NEXT STEP!! Determine if the creature is parented to an ACU; use the same logic to calculate data to add for both inside and outside
                    //NOTE!! creatures growing to maturity will potentially pose a big issue; I'll need to test if I can access MaturityTime easily or not

                    //Apply the new placeholder/default WaterParkCreatureData
                    wpc.data = ScriptableObject.CreateInstance<WaterParkCreatureData>();
                    logger.LogWarning($"(LiveMixin) Placeholder WaterParkCreatureData created!");
                }
            //}
            //else
            //{
                //ErrorMessage.AddError($"{techType} is not in the waterPark reference dictionary!");
            //}
        }

        //Populate WaterParkCreatureData with actual data, calculated from the size of the creature
        [HarmonyPatch(typeof(WaterParkCreature), nameof(WaterParkCreature.Start))]
        [HarmonyPostfix] //NOTE!! Creatures in containment will not trigger Creature.Start when loading in; they will only when released from the inventory; thus we use WaterParkCreature.Start
        public static void PopulateWPCData(WaterParkCreature __instance)
        {
            //Because many things use LiveMixin, we need to filter; using the WaterParkReference dictionary is perfect here
            TechType techType = CraftData.GetTechType(__instance.gameObject);

            if (WaterParkReference.ContainsKey(techType))
            {
                WaterParkCreature wpc = __instance;
                logger.LogWarning($"(WaterParkCreature) Size of {techType} = {GetSize(__instance.gameObject)}");

                logger.LogWarning($"(WaterParkCreature) {wpc.isInside}");
                logger.LogWarning($"(WaterParkCreature) Replacing placeholder {techType} WaterParkComponent data with real data!");

                //If creature is inside the alien containment, then it'll be smaller than its max size, and we need to work backwards to calculate WPC data
                //NOTE!! We specify if currentWaterPark isn't null, as if they are in inventory after being picked up from containment, it'll be a false positive
                if (wpc.isInside && wpc.currentWaterPark != null)
                {
                    //If creature is already mature, that means it's currently at its max size, so we calculate from here as normal
                    //if(wpc.isMature)
                    //{
                        logger.LogWarning($"(WaterParkCreature) {techType} is already inside alien containment! Calcuating original size!");

                        //By performing the calculation to get maxSize (x * 0.6), but in reverse (x / 0.6), we get our old size back
                        var initialSize = GetSize(__instance.gameObject) / 0.6f;
                        SetWaterParkData(ref wpc.data, initialSize);
                    //}
                    //If creature is not mature, then we need to use
                    //ERROR!! IsMature is not accessible at Pre or Post Start
                    //else
                    //{
                        logger.LogError($"{wpc.isMature}, {wpc.matureTime}, {wpc.data.outsideSize}, {wpc.data.maxSize}");
                    //}
                }
                //If creature is not in alien containment, then we just use its current size to calculate WPC data
                else
                {
                    logger.LogWarning($"(WaterParkCreature) {techType} is not inside alien containment! Using current size!");

                    //If the creature isn't in alien containment, this means we can just use its current size, as normal, to set the WaterParkCreatureData
                    var currentSize = GetSize(__instance.gameObject);
                    SetWaterParkData(ref wpc.data, currentSize);
                }
                logger.LogWarning($"(WaterParkCreature) Real WaterParkCreatureData created!");
            }
            else
            {
                ErrorMessage.AddError($"{techType} is not in the waterPark reference dictionary!");
            }
        }

        public static bool CheckPickupableComponent(GameObject creature, float modifier)
        {
            //Generate techtype to check the dictionary for creature's entry
            TechType techType = CraftData.GetTechType(creature);

            if(PickupableReference.ContainsKey(techType))
            {
                //NOTE!! Need to account for creatures inside WaterParks when calculating this, as they need to be able to be picked up still, but not when placed outside
                //The way to do this is to make IsPickupableOutside false in the WPC data; thus, might possibly be better to do all the checking there?

                //The size range within which a creature is made able to be picked up
                //Being outside of this range means the creature will have the Pickupable component removed, if it has one
                var (min, max) = PickupableReference[techType];

                //Whether the creature has a Pickupable component already or not
                bool componentExists = !(creature.GetComponent<Pickupable>() == null);

                GetInsideWaterPark(creature);

                if (modifier >= min && modifier <= max)
                {
                    //If creature is eligible for the component and doesn't have one, add it and return true
                    if (!componentExists)
                    {
                        creature.AddComponent<Pickupable>();
                        return true;
                    }
                }
                else
                {
                    //If creature is ineligable for the component and has one, remove it (and will return false by default)
                    if (componentExists)
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
                //DEBUG!!
                ErrorMessage.AddError($"Giving {techType} Pickupable anyway!");
                creature.AddComponent<Pickupable>();
                //DEBUG!!
                return true;
            }

            //Return false if any of the if statements are false
            return false;
        }

        public static void SetWaterParkData(ref WaterParkCreatureData data, float size)
        {
            //NOTE!! Each particular creature shares a WaterParkCreatureData (e.g. Hoopfish_WaterParkCreatureData)
            //This means two things; one, I can't change one SpineFish without changing the other, and two, I need to create one for the creatures that don't usually go in containment, like leviathans

            //2nd NOTE!! When loading into a save, with a creature such as a reaper in containment (a creature I added a waterparkcomponent to), the data is empty and null upon loading the save
            //Will likely need to look into repopulating the data every time I load in
            data.initialSize = size * 0.1f;
            data.maxSize = size * 0.6f;
            data.outsideSize = size;
        }

        public static bool GetInsideWaterPark(GameObject creature)
        {
            //DEBUG!!
            TechType techType = CraftData.GetTechType(creature);

            if(creature.GetComponentInParent<WaterPark>() != null)
            {
                logger.LogInfo($"{techType} is in a WaterPark");
                return true;
            }

            logger.LogInfo($"{techType} is not in a WaterPark");
            return false;
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
