using HarmonyLib;
using BepInEx.Logging;
using static CreatureConfigHealth.CreatureConfigHealthPlugin;
using System.Collections.Generic;
using UnityEngine;

namespace CreatureConfigHealth
{
    [HarmonyPatch]
    internal class CreatureConfigHealth
    {
        static readonly Dictionary<string, float> defaultHealthValues = new Dictionary<string, float>()
        {
            //Edible Fauna
            { "BladderfishHP",1F },
            { "BoomerangHP",1F },
            { "EyeyeHP",1F },
            { "GarryfishHP",1F },
            { "HolefishHP",1F },
            { "HoopfishHP",1F },
            { "HoverfishHP",1F },
            { "MagmarangHP",1F },
            { "OculusHP",1F },
            { "PeeperHP",20F },
            { "RedEyeyeHP",1F },
            { "ReginaldHP",1F },
            { "SpadefishHP",1F },
            { "SpinefishHP",1F },

            //Passive Fauna
            { "CrimsonRayHP",100F },
            { "CuddlefishHP",10000F },
            { "FloaterHP",40F },
            { "GasopodHP",1F },
            { "GhostrayHP",1F },
            { "JellyrayHP",1F },
            { "LavaLarvaHP",1F },
            { "RabbitRayHP",1F },
            { "SeaTreaderHP",1F },
            { "ShuttlebugHP",1F },
            { "SkyrayHP",100F },

            //Aggressive Fauna
            { "AmpeelHP",1F },
            { "BiterHP",10F },
            { "BleederHP",1F },
            { "BloodCrawlerHP",1F },
            { "BlighterHP",1F },
            { "BonesharkHP",1F },
            { "CaveCrawlerHP",1F },
            { "CrabsnakeHP",1F },
            { "CrabsquidHP",1F },
            { "CrashfishHP",1F },
            { "GhostLeviathanHP",1F },
            { "GhostLeviathanJuvenileHP",1F },
            { "LavaLizardHP",1F },
            { "MesmerHP",1F },
            { "ReaperHP",1F },
            { "RiverProwlerHP",1F },
            { "SandsharkHP",1F },
            { "SeaDragonHP",1F },
            { "StalkerHP", 1F },
            { "WarperHP", 1F }
        };

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPrefix]
        public static void PrefixCreatureStart(Creature __instance)
        {
            //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
            GameObject __creature = __instance.gameObject;

            TechType __techType = CraftData.GetTechType(__creature);

            //NOTE!! Best case, I can change default health of the creature prefabs, meaning health is changed to the default I set
            //Otherwise, could potnetially run into the issue of damage a fish, unload, come back and its healed
            //Granted, this would only be an issue for leviathans, and even then, how would player know they healed?

            switch (__techType)
            {
                case TechType.Biter:
                    ChangeHealth(__creature, config.BiterHP, "BiterHP");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeHealth(__creature, config.BloodCrawlerHP, "BloodCrawlerHP");
                    break;
                case TechType.Blighter:
                    ChangeHealth(__creature, config.BloodCrawlerHP, "BloodCrawlerHP");
                    break;
                case TechType.BoneShark:
                    ChangeHealth(__creature, config.BonesharkHP, "BonesharkHP");
                    break;
                case TechType.CaveCrawler:
                    ChangeHealth(__creature, config.CaveCrawlerHP, "CaveCrawlerHP");
                    break;
                case TechType.CrabSquid:
                    ChangeHealth(__creature, config.CrabsquidHP, "CrabsquidHP");
                    break;
                case TechType.Mesmer:
                    ChangeHealth(__creature, config.MesmerHP, "MesmerHP");
                    break;
                case TechType.SpineEel: //TechType for River Prowler
                    ChangeHealth(__creature, config.RiverProwlerHP, "RiverProwlerHP");
                    break;
                case TechType.Sandshark:
                    ChangeHealth(__creature, config.SandSharkHP, "SandsharkHP");
                    break;
                case TechType.SeaDragon:
                    ChangeHealth(__creature, config.SeaDragonLeviathanHP, "SeaDragonLeviathanHP");
                    break;
                case TechType.Stalker:
                    ChangeHealth(__creature, config.StalkerHP, "StalkerHP");
                    break;
                case TechType.Warper:
                    ChangeHealth(__creature, config.WarperHP, "WarperClawHP");
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage, or shoots a projectile; change case by case)
                //This includes; Bleeder, Crabsnake, Ghost, Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Shocker: //TechType for Ampeel
                    ChangeHealth(__creature, config.AmpeelHP, "AmpeelHP");
                    break;
                case TechType.Bleeder:
                    ChangeHealth(__creature, config.BleederHP, "BleederHP");
                    break;
                case TechType.Crabsnake:
                    ChangeHealth(__creature, config.CrabsnakeHP, "CrabsnakeHP");
                    break;
                case TechType.Crash: //TechType for Crashfish
                    ChangeHealth(__creature, config.CrashfishHP, "CrashfishHP");
                    break;
                case TechType.LavaLizard:
                    ChangeHealth(__creature, config.LavaLizardHP, "LavaLizardHP");
                    break;
                case TechType.GhostLeviathan:
                    ChangeHealth(__creature, config.GhostLeviathanHP, "GhostLeviathanHP");
                    break;
                case TechType.GhostLeviathanJuvenile:
                    ChangeHealth(__creature, config.GhostLeviathanJuvenileHP, "GhostLeviathanJuvenileHP");
                    break;
                case TechType.ReaperLeviathan:
                    ChangeHealth(__creature, config.ReaperLeviathanHP, "ReaperLeviathanHP");
                    break;
                case TechType.SeaTreader:
                    ChangeHealth(__creature, config.SeaTreaderLeviathanHP, "SeaTreaderLeviathanHP");
                    break;
                
            }
        }

        public static void ChangeHealth(GameObject __instance, float __customHPValue, string __defaultHPValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct health value to assign to creature; returns -1 if no such creature exists in the dictionary
            float __HPValueToAssign = CalculateHPToAssign(__customHPValue, __defaultHPValueKey);

            //Check if the method managed to calculate a health value to assign; -1 if it did not
            if (__HPValueToAssign != -1)
            {
                //Set health to new health value
                __instance.GetComponent<LiveMixin>().health = __HPValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning health
                logger.Log(LogLevel.Error, $"Failed setting {__techType} health");
            }
        }

        //Passed custom health value and default health value from dictionary; returns either the health value to assign, or -1 if there is no such key in the dictionary
        public static float CalculateHPToAssign(float __customHPValue, string __defaultHPValueKey)
        {
            //First, check if key for default health value actually exists in the dictionary
            if (defaultHealthValues.ContainsKey(__defaultHPValueKey))
            {
                //If it exists, assign the default health value to a variable
                float __defaultHPValue = defaultHealthValues[__defaultHPValueKey];

                //Store value to assign as new health for the selected creature's health
                //Default value is the default health for the creature's health
                float __HPValueToAssign = __defaultHPValue;

                //Obtain preset and determine which health value to assign according to the preset
                float __preset = config.HealthPreset;

                switch (__preset)
                {
                    //Custom, apply individual custom changes
                    case 1:
                        __HPValueToAssign = __customHPValue;
                        break;

                    //One-Hit, make all health values 1
                    case 2:
                        __HPValueToAssign = 1;
                        break;

                    //Damage Presets 3,4,5,6,7, multiply default health values by a percentage, based on the preset selected
                    //5 is Default, health value is reset to default
                    case float n when n >= 3 && n <= 7:
                        __HPValueToAssign = (__preset - 1) / 4 * __defaultHPValue;
                        break;

                    //Invulnerable, make all health values 100000
                    //NOTE!! Instead, what if I just turn on the immortal boolean under LiveMixin? Reefback has that set iirc?
                    //LOOK INTO IT!!
                    case 8:
                        __HPValueToAssign = 100000;
                        break;

                    default:
                        logger.Log(LogLevel.Error, $"Preset {__preset} not recognised!");
                        break;
                }

                //Return health value to assign
                return __HPValueToAssign;
            }
            else
            {
                //Return -1 if the calculation failed and the key did not exist in the default health dictionary
                logger.Log(LogLevel.Error, $"Default Health Value Key {__defaultHPValueKey} does not exist in the dictionary!");
                return -1.0f;
            }
        }
    }
}
