using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using UnityEngine;
using System;

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
                ErrorMessage.AddDebug($"Setting size of TechType  {techType}");

                float modifier = 1.0f;

                int sizeClass = GetCreatureSizeClass(techType);

                switch(sizeClass)
                {
                    case (int)SizeClass.Small:
                        modifier = GetSmallSizeModifier();
                        break;
                    case (int)SizeClass.Medium:
                        modifier = GetMedSizeModifier();
                        break;
                    case (int)SizeClass.Large:
                        modifier = GetLargeSizeModifier();
                        break;
                    default:
                        logger.LogWarning($"Error! Could not retrieve size class for TechType {techType}!");
                        break;
                }

                //NOTE!! If i use the reference array above, this switch statement would *only* be for unique changes made!
                //NOTE!! Can also use this for any changes unique to the creature we need to make, if any; probably jsut for leviathans
                switch (techType)
                {
                    case TechType.ReaperLeviathan:
                        //NOTE!! If I want to deal with velocity, will have to deal with leash movement of leviathans too
                        //NOTE!! Perhaps I can increase the speed of the leviathan, whilst inversely decreasing the turning speed of it, as it gets larger?
                        //Probably no need to increase the turning speed of smaller leviathans; they seem to get buggy when it's put above 3
                        //If they're faster larger, do I need to increase their leash distance a bit? Is that dangerous?
                        break;
                    default:
                        break;
                }

                //Once we've retrieved the modifier, apply the change to size, by the modifier
                ChangeSize(creature, modifier);
            }
        }

        //Just an easier way to read the sizeClass of the creatures
        public enum SizeClass { Small, Medium, Large }

        private static int GetCreatureSizeClass(TechType techType)
        {
            //Return -1 if no size class can be found in the reference array
            var size = -1;

            //Try to get the size class of the given TechType
            for(var i = 0; i < 3; i++)
            {
                for(var j = 0; j < CreatureSizeReference[i].Length; j++)
                {
                    if (CreatureSizeReference[i][j] == techType)
                    {
                        //We return i as the size class, as it refers to which of the 3 size arrays we found the TechType match in
                        //We use the SizeClass array in future references, for legibility
                        size = i;
                        break;
                    }
                }
                //No need to continue the loop if we've found our match
                if (size != -1) { break; }
            }

            return size;
        }

    private static float GetSmallSizeModifier()
        {
            //Return a random size between the min and max set for leviathan-size creatures
            System.Random rand = new System.Random();
            //Return an int value, between min and max values, both multiplied by 10
            //We both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            ErrorMessage.AddDebug($"Generating random size for small creature, between {config.SmallCreatureMinSize} and {config.SmallCreatureMaxSize}");
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
            ErrorMessage.AddDebug($"Generating random size for medium creature, between {config.MedCreatureMinSize} and {config.MedCreatureMaxSize}");
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
            ErrorMessage.AddDebug($"Generating random size for large creature, between {config.LargeCreatureMinSize} and {config.LargeCreatureMaxSize}");
            var modifierByTen = rand.Next((int)config.LargeCreatureMinSize * 10, (int)config.LargeCreatureMaxSize * 10);

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
