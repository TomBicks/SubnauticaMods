using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using UnityEngine;
using System;
using static CreatureConfigSize.CreatureConfigSize;

namespace CreatureConfigSize
{
    [HarmonyPatch]
    internal class CreatureConfigSize
    {
        //NOTE!! Whilst this won't make it more efficient, requiring more checks, surely making the code cleaner and more legible is better (can't imagine this would even remotely tank performance)
        //NOTE!! However, I won't be using it forever until I'm sure the switch case statement isn't more helpful, for things unique to each creature
        static readonly TechType[][] CreatureSizeReference = {
            new TechType[] {TechType.Biter, TechType.Bladderfish, TechType.Bleeder, TechType.Blighter, TechType.Shuttlebug, TechType.Boomerang, TechType.CaveCrawler, TechType.Crash, 
                TechType.Eyeye, TechType.Floater, TechType.GarryFish, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.LavaBoomerang, TechType.LavaLarva, TechType.Mesmer, 
                TechType.Oculus, TechType.Peeper, TechType.LavaEyeye, TechType.Reginald, TechType.Skyray, TechType.Spadefish, TechType.Spinefish, TechType.Jumper},
            new TechType[] {TechType.Shocker, TechType.BoneShark, TechType.Crabsnake, TechType.CrabSquid, TechType.Cutefish, TechType.GhostRayRed, TechType.Gasopod, TechType.GhostRayBlue, 
                TechType.Jellyray, TechType.LavaLizard, TechType.RabbitRay, TechType.SpineEel, TechType.Sandshark, TechType.SeaEmperorBaby, TechType.Stalker, TechType.Warper},
            new TechType[] {TechType.GhostLeviathan, TechType.GhostLeviathanJuvenile, TechType.ReaperLeviathan, TechType.SeaDragon, TechType.SeaEmperor, TechType.SeaEmperorJuvenile, 
                TechType.SeaTreader}
        };

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPostfix] //Postfix means less chance of missing setting any creatures' size
        public static void PostCreatureStart(Creature __instance)
        {
            //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
            GameObject creature = __instance.gameObject;

            TechType techType = CraftData.GetTechType(creature);

            if(techType != TechType.None)
            {
                logger.LogDebug($"Setting size of TechType {techType}");
                ErrorMessage.AddDebug($"Setting size of TechType {techType}");

                //Generate a modifier based on the creature's size class, retrieved from the CreatureSizeReference array
                float modifier = GetCreatureSizeModifier(techType); ;

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
                        //creature.gameObject.GetComponent<FMOD_CustomEmitter>().evt.
                        #endregion
                        creature.AddComponent<Pickupable>();
                        break;
                    default:
                        break;
                }

                //Once we've retrieved the modifier, apply the change to size, by the modifier
                ChangeSize(creature, modifier);
            }
            else
            {
                logger.LogWarning($"Error! Creature {__instance.name} has no TechType!");
            }
        }

        //Just an easier way to read the sizeClass of the creatures
        public enum SizeClass { None, Small, Medium, Large }

        private static float GetCreatureSizeModifier(TechType techType)
        {
            //Return 0 if no size class can be found in the reference array
            var sizeClass = SizeClass.None;

            //Return a default modifier of 1 if no size class can be found in the reference array
            var modifier = 1.0f;

            //Try to get the size class of the given TechType
            for (var i = 0; i < 3; i++)
            {
                for(var j = 0; j < CreatureSizeReference[i].Length; j++)
                {
                    if (CreatureSizeReference[i][j] == techType)
                    {
                        //We return i (+1 as None is 0) as the size class, as it refers to which of the 3 size arrays we found the TechType match in
                        //We use the SizeClass array in future references, for legibility
                        sizeClass = (SizeClass)(i+1); //This turns the int result into its appropriate enum counterpart; e.g. 1 becomes SizeClass.Small
                        break;
                    }
                }
                //If we found a size class, generate a modifier for it, then break from the loop
                if (sizeClass != SizeClass.None)
                {
                    switch (sizeClass)
                    {
                        case SizeClass.Small:
                            modifier = GetSmallSizeModifier();
                            break;
                        case SizeClass.Medium:
                            modifier = GetMedSizeModifier();
                            break;
                        case SizeClass.Large:
                            modifier = GetLargeSizeModifier();
                            break;
                    }
                }
            }

            if(sizeClass == SizeClass.None)
            {
                logger.LogWarning($"Error! Could not retrieve size class for TechType {techType}!");
            }

            return modifier;
        }

    private static float GetSmallSizeModifier()
        {
            //Return a random size between the min and max set for leviathan-size creatures
            System.Random rand = new System.Random();
            //Return an int value, between min and max values, both multiplied by 10
            //We both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            var modifierByTen = rand.Next((int)config.SmallCreatureMinSize * 10, (int)config.SmallCreatureMaxSize * 10);
            //Divide the modifier back down by 10, meaning we have a modifier to 1 decimal place.
            var modifier = (float)modifierByTen / 10;

            return modifier;
        }

        private static float GetMedSizeModifier()
        {
            //Return a random size between the min and max set for leviathan-size creatures
            System.Random rand = new System.Random();
            //Return an int value, between min and max values, both multiplied by 10
            //We both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            var modifierByTen = rand.Next((int)config.MedCreatureMinSize * 10, (int)config.MedCreatureMaxSize * 10);

            //Divide the modifier back down by 10, meaning we have a modifier to 1 decimal place.
            var modifier = (float)modifierByTen / 10;

            return modifier;
        }

        private static float GetLargeSizeModifier()
        {
            //Return a random size between the min and max set for leviathan-size creatures
            System.Random rand = new System.Random();
            //Return an int value, between min and max values, both multiplied by 10
            //We both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            var modifierByTen = rand.Next((int)(config.LargeCreatureMinSize * 10), (int)(config.LargeCreatureMaxSize * 10));

            //Divide the modifier back down by 10, meaning we have a modifier to 1 decimal place.
            var modifier = (float)modifierByTen / 10;

            return modifier;
        }

        private static void ChangeSize(GameObject creature, float modifier)
        {
            creature.transform.localScale = new Vector3(modifier, modifier, modifier);
        }
    }
}
