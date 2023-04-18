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
            { "BiterDmg",7F }
        };

        /*[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void PostfixStart(Player __instance)
        {
            // Check to see if this is the player
            if (__instance.GetType() == typeof(Player))
            {
                //Populate default damage values dictionary
                defaultDamageValues.Add("BiterDmg",7F);
            }
        }*/

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPrefix]
        public static void PrefixCreatureStart(Creature __instance)
        {
            #region Attempt 1; Several Nested If Statements
            /*if (__instance.gameObject.GetComponent<CrabSnake>())
            {
                logger.Log(LogLevel.Info, $"Found Crabsnake; setting damage to {config.CrabsnakeDmg}");
                if(__instance.gameObject.GetComponent<CrabsnakeMeleeAttack>())
                {
                    logger.Log(LogLevel.Info, "Found CrabsnakeMeleeAttack component");
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().biteDamage = 1f;
                }
            }
            if (__instance.gameObject.GetComponent<Biter>())
            {
                logger.Log(LogLevel.Info, "Found Biter");
                if (__instance.gameObject.GetComponent<MeleeAttack>())
                {
                    logger.Log(LogLevel.Info, $"Found MeleeAttack component; setting damage to {config.BiterDmg}");
                    __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = config.BiterDmg;
                }
            }*/
            #endregion

            #region Attempt 2; getting TechType and running the result through a switch case
            TechType __techType = CraftData.GetTechType(__instance.gameObject);
            //logger.Log(LogLevel.Info, $"Creature TechType = {__techType}");
            logger.Log(LogLevel.Info, $"Creature TechType = {defaultDamageValues["BiterDmg"]}");

            switch (config.DamagePreset)
            {
                //Default, apply individual changes
                case 4:
                    switch(__techType)
                    {
                        //Handle generic cases (just a MeleeAttack component; change biteDamage)
                        case TechType.Biter:
                            ChangeGenericMeleeAttack(__instance, config.BiterDmg);
                            break;
                        case TechType.Bleeder:
                            break;
                        case TechType.Shuttlebug: //Blood Crawler
                            break;
                        case TechType.BoneShark:
                            ChangeGenericMeleeAttack(__instance, config.BonesharkDmg);
                            break;
                        case TechType.CaveCrawler:
                            break;
                        case TechType.CrabSquid:
                            break;
                        case TechType.LavaLizard:
                            break;
                        case TechType.Mesmer:
                            break;
                        case TechType.Sandshark:
                            ChangeGenericMeleeAttack(__instance, config.SandSharkDmg);
                            break;
                        case TechType.Stalker:
                            break;

                        //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage; change case by case)
                        //This includes; Crabsnake, Ghost, Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper 
                        case TechType.Crabsnake:
                            logger.Log(LogLevel.Info, $"Found Crabsnake; setting damage to {config.CrabsnakeDmg}");
                            __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().biteDamage = config.CrabsnakeDmg;
                            break;
                    }
                    break;

                //Sandbox, make all damage values 1
                case 1:
                    break;

                //Sudden Death, make all damage values 1000?
                case 7:
                    break;

                //Damage Presets 2,3,5,6, multiply default damage values by a percentage
                default:
                    break;
            }
            #endregion
        }

        public static void ChangeGenericMeleeAttack(Creature __instance, float __damageValue)
        {
            //DEBUG CODE; prints creature type and damage set
            TechType __techType = CraftData.GetTechType(__instance.gameObject);
            logger.Log(LogLevel.Info, $"Setting {__techType} biteDamage to {__damageValue}");

            //Set biteDamage to new damage value
            __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = __damageValue;
        }


        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
        [HarmonyPrefix]
        public static void PrefixTakeDamage(LiveMixin __instance)
        {
            //If all else fails; hook into dealer and see what they are and retroactively change the damage here?
            //https://harmony.pardeike.net/articles/patching-injections.html
        }
    }

    [Serializable]
    public class SpawnData
    {
        public bool AlwaysRandomized = false;
        public float ReaperSpawnIntensity = 3;
        public float GhostSpawnIntensity = 3;
        public float[][] ReaperCoords =
        {
            new float[]{ 120, -40, -568 }, //Grassy Plateaus, South
            new float[]{ 1407, -190, 584 }, //Bulb Zone, East-North-East
            new float[]{ 278, -175, 1398 }, //Mountains, North
            new float[]{ -811, -240, -1240 }, //Grand Reef, South-South-West
            new float[]{ -442, -132, -912 }, //Grand Reef, South
            new float[]{ -310, -45, 92 }, //Kelp Forest, West
            new float[]{ 190, -52, 477 }, //Kelp Forest, North
            new float[]{ 500, -98, 318 }, //Mushroom Forest, East
            new float[]{ 680, -85, 331 }, //Mushroom Forest, East
            new float[]{ -172, -70, -781 }, //Crag Field, South
            new float[]{ -692, -130, -725 }, //Sparse Reef, South-West
            new float[]{ -745, -80, -1050 }, //Grand Reef, South-South-West (Under Floating Island)
            new float[]{ -278, -30, -621 }, //Kelp Forest, South
            new float[]{ -295, -45, -350 }, //Grassy Plateaus, South-West
            new float[]{ -516, -110, 531 }, //Mushroom Forest, North-West
            new float[]{ -815, -68, 316 }, //Mushroom Forest, West-North-West
            new float[]{ -531, -60, -175 }, //Grassy Plateaus, West
            new float[]{ -250, -142, 906 }, //Underwater Islands, North
            new float[]{ -1122, -113, 710 }, //Mushroom Forest, North-West
            new float[]{ -1190, -102, -527 }, //Sea Treader's Path, West-South-West
            new float[]{ -754, -102, 1334 }, //Blood Kelp Zone, North-North-West
            new float[]{ 432, -65, 690 }, //Kelp Forest, North-North-East
            new float[]{ 383, -60, 40 } //Grassy Plateaus, East
        };
        public float[][] GhostCoordsAndType =
        {
            new float[]{ -284, -293, 1100, 1 }, //Adult, Underwater Islands, North
            new float[]{ 1065, -211, 466, 1 }, //Adult, Bulb Zone, East-North-East
            new float[]{ 876, -122, 881, 1 }, //Adult, Bulb Zone, North-East
            new float[]{ -28, -318, 1296, 1 }, //Adult, Mountains, North
            new float[]{ 10, -219, -220, 2 }, //Juvenile, Jellyshroom Cave, South
            new float[]{ -396, -350, -925, 2 }, //Juvenile, Grand Reef, South
            new float[]{ -958, -300, -540, 2 }, //Juvenile, Blood Kelp Trench, South-West
            new float[]{ -988, -885, 400, 2 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new float[]{ -695, -478, -993, 2 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new float[]{ -618, -213, -82, 2 }, //Juvenile, Jellyshroom Cave, West
            new float[]{ -34, -400, 926, 2 }, //Juvenile, Underwater Islands, North
            new float[]{ -196, -436, 1056, 2 }, //Juvenile, Underwater Islands, North
            new float[]{ 1443, -260, 883, 2 }, //Juvenile, Bulb Zone, North-East
            new float[]{ 1075, -475, 944, 2 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }
}
