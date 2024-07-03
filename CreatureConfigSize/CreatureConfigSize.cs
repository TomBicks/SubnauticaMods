using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using UnityEngine;
using System;

namespace CreatureConfigSize
{
    [HarmonyPatch]
    internal class CreatureConfigSize
    {
        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPrefix]
        public static void PrefixCreatureStart(Creature __instance)
        {
            //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
            GameObject creature = __instance.gameObject;

            TechType techType = CraftData.GetTechType(creature);

            logger.LogDebug($"Setting size of TechType {techType}");
            ErrorMessage.AddDebug($"Setting size of TechType  {techType}");

            switch (techType)
            {
                case TechType.ReaperLeviathan:
                    //NOTE!! If I want to deal with velocity, will have to deal with leash movement of leviathans too
                    //NOTE!! Perhaps I can increase the speed of the leviathan, whilst inversely decreasing the turning speed of it, as it gets larger?
                    //Probably no need to increase the turning speed of smaller leviathans; they seem to get buggy when it's put above 3
                    //If they're faster larger, do I need to increase their leash distance a bit? Is that dangerous?
                    var modifier = GetLargeSizeModifier();
                    logger.LogDebug($"Setting Reaper to {modifier}*size");
                    ErrorMessage.AddDebug($"Setting Reaper to {modifier}*size");
                    creature.transform.localScale = new Vector3(modifier, modifier, modifier);
                    break;
            }
        }

        private static float GetLargeSizeModifier()
        {
            //Return a random size between the min and max set for leviathan-size creatures
            System.Random rand = new System.Random();
            //Return an int value, between min and max values, both multiplied by 10
            //We both by 10, so that we can get the numbers unbetween 1 & 2, or 10 and 20 here
            //We'll divide them back down by 10 after we've gotten a random integer
            //var modifierByTen = rand.Next((int)config.LargeCreatureMinSize * 10, (int)config.LargeCreatureMaxSize * 10);
            var modifierByTen = rand.Next((int)(0.2f*10), (int)(4.5f*10));
            ErrorMessage.AddDebug($"{modifierByTen}");
            //Divide the modifier back down by 10, meaning we have a modifier to 1 decimal place.
            var modifier = (float)modifierByTen / 10;
            ErrorMessage.AddDebug($"{modifier}");


            //This returns a value between 0.1 and 1
            //var roundResult = Math.Round(rand.NextDouble(), 1, MidpointRounding.AwayFromZero);
            //ErrorMessage.AddDebug($"{roundResult}");

            return modifier;
        }

        private static void ChangeSize(GameObject creature)
        {

        }
    }
}
