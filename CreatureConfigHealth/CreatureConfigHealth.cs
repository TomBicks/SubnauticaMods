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
            { "BladderfishHP",30F },
            { "BoomerangHP",30F },
            { "EyeyeHP",30F },
            { "GarryfishHP",30F },
            { "HolefishHP",30F },
            { "HoopfishHP",20F },
            { "HoverfishHP",30F },
            { "MagmarangHP",30F },
            { "OculusHP",20F },
            { "PeeperHP",20F },
            { "RedEyeyeHP",25F },
            { "ReginaldHP",30F },
            { "SpadefishHP",30F },
            { "SpinefishHP",20F },

            //Passive Fauna
            { "CrimsonRayHP",100F },
            { "CuddlefishHP",10000F },
            { "FloaterHP",40F },
            { "GasopodHP",300F },
            { "GhostrayHP",100F },
            { "JellyrayHP",100F },
            { "LavaLarvaHP",100F },
            { "RabbitRayHP",100F },
            { "SeaTreaderLeviathanHP",3000F },
            { "ShuttlebugHP",50F },
            { "SkyrayHP",100F },

            //Aggressive Fauna
            { "AmpeelHP",3000F },
            { "BiterHP",10F },
            { "BleederHP",10F }, //Don't think I can truly make these things immortal; death sentence if they latch on
            { "BlighterHP",10F },
            { "BloodCrawlerHP",50F },
            { "BonesharkHP",200F },
            { "CaveCrawlerHP",50F },
            { "CrabsnakeHP",300F },
            { "CrabsquidHP",500F },
            { "CrashfishHP",25F },
            { "GhostLeviathanHP",8000F },
            { "GhostLeviathanJuvenileHP",8000F },
            { "LavaLizardHP",200F },
            { "MesmerHP",100F },
            { "ReaperLeviathanHP",5000F },
            { "RiverProwlerHP",200F },
            { "SandsharkHP",250F },
            { "SeaDragonLeviathanHP",5000F },
            { "StalkerHP", 300F },
            { "WarperHP", 100F }
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

            //If set to apply health presets to aggressive fauna only, ignore this section of fauna; the edible and passive
            if(!config.ApplyPresetsToAggressiveOnly)
            {
                switch (__techType)
                {
                    //Edible Fauna
                    case TechType.Bladderfish:
                        ChangeHealth(__creature, config.BladderfishHP, "BladderfishHP");
                        break;
                    case TechType.Boomerang:
                        ChangeHealth(__creature, config.BoomerangHP, "BoomerangHP");
                        break;
                    case TechType.Eyeye:
                        ChangeHealth(__creature, config.EyeyeHP, "EyeyeHP");
                        break;
                    case TechType.GarryFish:
                        ChangeHealth(__creature, config.GarryfishHP, "GarryfishHP");
                        break;
                    case TechType.HoleFish:
                        ChangeHealth(__creature, config.HolefishHP, "HolefishHP");
                        break;
                    case TechType.Hoopfish:
                        ChangeHealth(__creature, config.HoopfishHP, "HoopfishHP");
                        break;
                    case TechType.Hoverfish:
                        ChangeHealth(__creature, config.HoverfishHP, "HoverfishHP");
                        break;
                    case TechType.LavaBoomerang: //TechType for Magmarang
                        ChangeHealth(__creature, config.MagmarangHP, "MagmarangHP");
                        break;
                    case TechType.Oculus:
                        ChangeHealth(__creature, config.OculusHP, "OculusHP");
                        break;
                    case TechType.Peeper:
                        ChangeHealth(__creature, config.PeeperHP, "PeeperHP");
                        break;
                    case TechType.LavaEyeye: //TechType for Red Eyeye
                        ChangeHealth(__creature, config.RedEyeyeHP, "RedEyeyeHP");
                        break;
                    case TechType.Reginald:
                        ChangeHealth(__creature, config.ReginaldHP, "ReginaldHP");
                        break;
                    case TechType.Spadefish:
                        ChangeHealth(__creature, config.SpadefishHP, "SpadefishHP");
                        break;
                    case TechType.Spinefish:
                        ChangeHealth(__creature, config.SpinefishHP, "SpinefishHP");
                        break;

                    //Passive Fauna
                    case TechType.GhostRayRed: //TechType for Crimson Ray
                        ChangeHealth(__creature, config.CrimsonRayHP, "CrimsonRayHP");
                        break;
                    case TechType.Cutefish: //TechType for Cuddlefish
                                            //If option to make Cuddlefish invulnerable is ticked, ignore changing the health value of the Cuddlefish
                        if (config.CuddlefishInvunerable)
                        {
                            MakeInvulnerable(__creature);
                        }
                        else
                        {
                            ChangeHealth(__creature, config.CuddlefishHP, "CuddlefishHP");
                        }
                        break;
                    case TechType.Floater:
                        ChangeHealth(__creature, config.FloaterHP, "FloaterHP");
                        break;
                    case TechType.Gasopod:
                        ChangeHealth(__creature, config.GasopodHP, "GasopodHP");
                        break;
                    case TechType.GhostRayBlue: //TechType for Ghostray
                        ChangeHealth(__creature, config.GhostrayHP, "GhostrayHP");
                        break;
                    case TechType.Jellyray:
                        ChangeHealth(__creature, config.JellyrayHP, "JellyrayHP");
                        break;
                    case TechType.LavaLarva:
                        ChangeHealth(__creature, config.LavaLarvaHP, "LavaLarvaHP");
                        break;
                    case TechType.RabbitRay:
                        ChangeHealth(__creature, config.RabbitRayHP, "RabbitRayHP");
                        break;
                    case TechType.SeaTreader:
                        ChangeHealth(__creature, config.SeaTreaderLeviathanHP, "SeaTreaderLeviathanHP");
                        break;
                    case TechType.Jumper: //TechType for Shuttlebug; not to be confused with TechType.Shuttlebug which is for Blood Crawlers
                        ChangeHealth(__creature, config.ShuttlebugHP, "ShuttlebugHP");
                        break;
                    case TechType.Skyray:
                        ChangeHealth(__creature, config.SkyrayHP, "SkyrayHP");
                        break;
                }
            }
            
            switch (__techType)
            {
                //Aggressive Fauna
                case TechType.Shocker: //TechType for Ampeel
                    ChangeHealth(__creature, config.AmpeelHP, "AmpeelHP");
                    break;
                case TechType.Biter:
                    ChangeHealth(__creature, config.BiterHP, "BiterHP");
                    break;
                case TechType.Bleeder:
                    ChangeHealth(__creature, config.BleederHP, "BleederHP");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeHealth(__creature, config.BloodCrawlerHP, "BloodCrawlerHP");
                    break;
                case TechType.Blighter:
                    ChangeHealth(__creature, config.BlighterHP, "BlighterHP");
                    break;
                case TechType.BoneShark:
                    ChangeHealth(__creature, config.BonesharkHP, "BonesharkHP");
                    break;
                case TechType.CaveCrawler:
                    ChangeHealth(__creature, config.CaveCrawlerHP, "CaveCrawlerHP");
                    break;
                case TechType.Crabsnake:
                    ChangeHealth(__creature, config.CrabsnakeHP, "CrabsnakeHP");
                    break;
                case TechType.CrabSquid:
                    ChangeHealth(__creature, config.CrabsquidHP, "CrabsquidHP");
                    break;
                case TechType.Crash: //TechType for Crashfish
                    ChangeHealth(__creature, config.CrashfishHP, "CrashfishHP");
                    break;
                case TechType.GhostLeviathan:
                    ChangeHealth(__creature, config.GhostLeviathanHP, "GhostLeviathanHP");
                    break;
                case TechType.GhostLeviathanJuvenile:
                    ChangeHealth(__creature, config.GhostLeviathanJuvenileHP, "GhostLeviathanJuvenileHP");
                    break;
                case TechType.LavaLizard:
                    ChangeHealth(__creature, config.LavaLizardHP, "LavaLizardHP");
                    break;
                case TechType.Mesmer:
                    ChangeHealth(__creature, config.MesmerHP, "MesmerHP");
                    break;
                case TechType.ReaperLeviathan:
                    ChangeHealth(__creature, config.ReaperLeviathanHP, "ReaperLeviathanHP");
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
            }
        }

        public static void ChangeHealth(GameObject __instance, float __customHPValue, string __defaultHPValueKey)
        {
            //If preset is set to 'Invulnerable', then health doesn't matter and we can skip everything else
            if (config.HealthPreset == 8)
            {
                MakeInvulnerable(__instance);
            }
            else
            {
                //DEBUG CODE; retrieves creature type
                TechType __techType = CraftData.GetTechType(__instance);

                //Calculate correct health value to assign to creature; returns -1 if no such creature exists in the dictionary
                float __HPValueToAssign = CalculateHPToAssign(__customHPValue, __defaultHPValueKey);

                //Check if the method managed to calculate a health value to assign; -1 if it did not
                if (__HPValueToAssign != -1)
                {
                    //DEBUG CODE; prints info; remove WHEN DONE
                    logger.Log(LogLevel.Info, $"Setting {__techType} health to {__HPValueToAssign}");

                    //Set health to new health value
                    __instance.GetComponent<LiveMixin>().health = __HPValueToAssign;
                }
                else
                {
                    //DEBUG CODE; prints failure in assigning health
                    logger.Log(LogLevel.Error, $"Failed setting {__techType} health");
                }
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

        //Makes the creature invulnerable, thereby ignoring any damage it takes
        public static void MakeInvulnerable (GameObject __instance)
        {
            __instance.GetComponent<LiveMixin>().invincible = true;
        }
    }
}
