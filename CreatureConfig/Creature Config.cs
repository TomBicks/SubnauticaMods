using HarmonyLib;
using System;
using System.IO;
using BepInEx.Logging;
using SMLHelper.V2.Handlers;
using static CreatureConfig.CreatureConfigPlugin;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CreatureConfig
{
    [HarmonyPatch]
    internal class CreatureConfig
    {
        static Dictionary<string, float> defaultDamageValues = new Dictionary<string, float>()
        {
            { "BiterDmg",7F },
            { "SandsharkDmg",30F },
            { "StalkerDmg", 30F }
        };

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPrefix]
        public static void PrefixCreatureStart(Creature __instance)
        {
            #region Attempt 2; getting TechType and running the result through a switch case
            TechType __techType = CraftData.GetTechType(__instance.gameObject);

            switch(__techType)
            {
                //Handle generic cases (just a MeleeAttack component; change biteDamage)
                case TechType.Biter:
                    ChangeGenericMeleeAttack(__instance, config.BiterDmg, defaultDamageValues["BiterDmg"]);
                    break;
                case TechType.Bleeder:
                    ChangeGenericMeleeAttack(__instance, config.BleederDmg, defaultDamageValues["BleederDmg"]);
                    break;
                case TechType.Shuttlebug: //Blood Crawler
                    ChangeGenericMeleeAttack(__instance, config.BloodCrawlerDmg, defaultDamageValues["BloodCrawlerDmg"]);
                    break;
                case TechType.BoneShark:
                    ChangeGenericMeleeAttack(__instance, config.BonesharkDmg, defaultDamageValues["BonesharkDmg"]);
                    break;
                case TechType.CaveCrawler:
                    ChangeGenericMeleeAttack(__instance, config.CaveCrawlerDmg, defaultDamageValues["CaveCrawlerDmg"]);
                    break;
                case TechType.CrabSquid:
                    ChangeGenericMeleeAttack(__instance, config.CrabsquidDmg, defaultDamageValues["CrabsquidDmg"]);
                    break;
                case TechType.LavaLizard:
                    //ChangeGenericMeleeAttack(__instance, config.Lava, defaultDamageValues["BleederDmg"]);
                    break;
                case TechType.Mesmer:
                    ChangeGenericMeleeAttack(__instance, config.MesmerDmg, defaultDamageValues["MesmerDmg"]);
                    break;
                case TechType.Sandshark:
                    ChangeGenericMeleeAttack(__instance, config.SandSharkDmg, defaultDamageValues["SandsharkDmg"]);
                    break;
                case TechType.Stalker:
                    ChangeGenericMeleeAttack(__instance, config.StalkerDmg, defaultDamageValues["StalkerDmg"]);
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage; change case by case)
                //This includes; Crabsnake, Ghost, Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper 
                case TechType.Crabsnake:
                    logger.Log(LogLevel.Info, $"Found Crabsnake; setting damage to {config.CrabsnakeDmg}");
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().biteDamage = config.CrabsnakeDmg;
                    break;
 
            }
            #endregion
        }

        public static void ChangeGenericMeleeAttack(Creature __instance, float __customDmgValue, float __defaultDmgValue)
        {
            //Store value to assign as new biteDamage for the selected creature; default value is the default biteDamge for the creature
            float __dmgValueToAssign = __defaultDmgValue;

            //Obtain preset and determine which damage value to assign according to the preset
            float __preset = config.DamagePreset;
            logger.Log(LogLevel.Info, $"Preset = {__preset}");

            switch(__preset)
            {
                //Custom, apply individual custom changes
                case 1:
                    __dmgValueToAssign = __customDmgValue;
                    break;

                //Sandbox, make all damage values 1
                case 2:
                    __dmgValueToAssign = 1;
                    break;

                //Default, keep default value and break out of switch statement
                case 5:
                    break;

                //Sudden Death, make all damage values 1000?
                case 8:
                    __dmgValueToAssign = 1000;
                    break;

                //Damage Presets 3,4,6,7, multiply default damage values by a percentage, based on the preset selected
                default:
                    __dmgValueToAssign = ((__preset - 1) / 4) * __defaultDmgValue;
                    break;
            }

            //DEBUG CODE; prints creature type and damage assigned
            TechType __techType = CraftData.GetTechType(__instance.gameObject);
            logger.Log(LogLevel.Info, $"Setting {__techType} biteDamage to {__dmgValueToAssign}");

            //Set biteDamage to new damage value
            __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = __dmgValueToAssign;
        }


        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
        [HarmonyPrefix]
        public static void PrefixTakeDamage(LiveMixin __instance)
        {
            //If all else fails; hook into dealer and see what they are and retroactively change the damage here?
            //https://harmony.pardeike.net/articles/patching-injections.html
        }
    }
}
