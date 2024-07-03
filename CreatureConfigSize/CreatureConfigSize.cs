using HarmonyLib;
using static CreatureConfigSize.CreatureConfigSizePlugin;
using UnityEngine;

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
                    var modifier = 4.0f;
                    logger.LogDebug($"Setting Reaper to {modifier}*size");
                    ErrorMessage.AddDebug($"Setting Reaper to {modifier}*size");
                    creature.transform.localScale = new Vector3(modifier, modifier, modifier);
                    break;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPrefix]
        public static void PostfixPlayer(Player __instance)
        {
            logger.LogDebug($"Testing");
            ErrorMessage.AddDebug($"Testing");
        }
    }
}
