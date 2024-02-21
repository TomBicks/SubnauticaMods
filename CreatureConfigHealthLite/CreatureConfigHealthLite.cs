using HarmonyLib;
using BepInEx.Logging;
using static CreatureConfigHealthLite.CreatureConfigHealthLitePlugin;
using System.Collections.Generic;
using UnityEngine;

namespace CreatureConfigHealthLite
{
    [HarmonyPatch]
    internal class CreatureConfigHealthLite
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
            { "BleederHP",10F },
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

            //If set to apply health presets to aggressive fauna only, ignore this section of fauna; the edible and passive
            if (!config.ApplyPresetsToAggressiveOnly)
            {
                switch (__techType)
                {
                    //Edible Fauna
                    case TechType.Bladderfish:
                        ChangeHealth(__creature, "BladderfishHP");
                        break;
                    case TechType.Boomerang:
                        ChangeHealth(__creature, "BoomerangHP");
                        break;
                    case TechType.Eyeye:
                        ChangeHealth(__creature, "EyeyeHP");
                        break;
                    case TechType.GarryFish:
                        ChangeHealth(__creature, "GarryfishHP");
                        break;
                    case TechType.HoleFish:
                        ChangeHealth(__creature, "HolefishHP");
                        break;
                    case TechType.Hoopfish:
                        ChangeHealth(__creature, "HoopfishHP");
                        break;
                    case TechType.Hoverfish:
                        ChangeHealth(__creature, "HoverfishHP");
                        break;
                    case TechType.LavaBoomerang: //TechType for Magmarang
                        ChangeHealth(__creature, "MagmarangHP");
                        break;
                    case TechType.Oculus:
                        ChangeHealth(__creature, "OculusHP");
                        break;
                    case TechType.Peeper:
                        ChangeHealth(__creature, "PeeperHP");
                        break;
                    case TechType.LavaEyeye: //TechType for Red Eyeye
                        ChangeHealth(__creature, "RedEyeyeHP");
                        break;
                    case TechType.Reginald:
                        ChangeHealth(__creature, "ReginaldHP");
                        break;
                    case TechType.Spadefish:
                        ChangeHealth(__creature, "SpadefishHP");
                        break;
                    case TechType.Spinefish:
                        ChangeHealth(__creature, "SpinefishHP");
                        break;

                    //Passive Fauna
                    case TechType.GhostRayRed: //TechType for Crimson Ray
                        ChangeHealth(__creature, "CrimsonRayHP");
                        break;
                    case TechType.Cutefish: //TechType for Cuddlefish
                        //If option to make Cuddlefish invulnerable is ticked, ignore changing the health value of the Cuddlefish
                        if (config.CuddlefishInvunerable)
                        {
                            MakeInvulnerable(__creature);
                        }
                        else
                        {
                            ChangeHealth(__creature, "CuddlefishHP");
                        }
                        break;
                    case TechType.Floater:
                        ChangeHealth(__creature, "FloaterHP");
                        break;
                    case TechType.Gasopod:
                        ChangeHealth(__creature, "GasopodHP");
                        break;
                    case TechType.GhostRayBlue: //TechType for Ghostray
                        ChangeHealth(__creature, "GhostrayHP");
                        break;
                    case TechType.Jellyray:
                        ChangeHealth(__creature, "JellyrayHP");
                        break;
                    case TechType.LavaLarva:
                        ChangeHealth(__creature, "LavaLarvaHP");
                        break;
                    case TechType.RabbitRay:
                        ChangeHealth(__creature, "RabbitRayHP");
                        break;
                    case TechType.SeaTreader:
                        ChangeHealth(__creature, "SeaTreaderLeviathanHP");
                        break;
                    case TechType.Jumper: //TechType for Shuttlebug; not to be confused with TechType.Shuttlebug which is for Blood Crawlers
                        ChangeHealth(__creature, "ShuttlebugHP");
                        break;
                    case TechType.Skyray:
                        ChangeHealth(__creature, "SkyrayHP");
                        break;
                }
            }

            switch (__techType)
            {
                //Aggressive Fauna
                case TechType.Shocker: //TechType for Ampeel
                    ChangeHealth(__creature, "AmpeelHP");
                    break;
                case TechType.Biter:
                    ChangeHealth(__creature, "BiterHP");
                    break;
                case TechType.Bleeder:
                    //If option to exclude Bleeder from health changes is ticked, skip the Bleeder
                    if (!config.ExcludeBleeder)
                    {
                        ChangeHealth(__creature, "BleederHP");
                    }
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeHealth(__creature, "BloodCrawlerHP");
                    break;
                case TechType.Blighter:
                    ChangeHealth(__creature, "BlighterHP");
                    break;
                case TechType.BoneShark:
                    ChangeHealth(__creature, "BonesharkHP");
                    break;
                case TechType.CaveCrawler:
                    ChangeHealth(__creature, "CaveCrawlerHP");
                    break;
                case TechType.Crabsnake:
                    ChangeHealth(__creature, "CrabsnakeHP");
                    break;
                case TechType.CrabSquid:
                    ChangeHealth(__creature, "CrabsquidHP");
                    break;
                case TechType.Crash: //TechType for Crashfish
                    ChangeHealth(__creature, "CrashfishHP");
                    break;
                case TechType.GhostLeviathan:
                    ChangeHealth(__creature, "GhostLeviathanHP");
                    break;
                case TechType.GhostLeviathanJuvenile:
                    ChangeHealth(__creature, "GhostLeviathanJuvenileHP");
                    break;
                case TechType.LavaLizard:
                    ChangeHealth(__creature, "LavaLizardHP");
                    break;
                case TechType.Mesmer:
                    ChangeHealth(__creature, "MesmerHP");
                    break;
                case TechType.ReaperLeviathan:
                    ChangeHealth(__creature, "ReaperLeviathanHP");
                    break;
                case TechType.SpineEel: //TechType for River Prowler
                    ChangeHealth(__creature, "RiverProwlerHP");
                    break;
                case TechType.Sandshark:
                    ChangeHealth(__creature, "SandsharkHP");
                    break;
                case TechType.SeaDragon:
                    ChangeHealth(__creature, "SeaDragonLeviathanHP");
                    break;
                case TechType.Stalker:
                    ChangeHealth(__creature, "StalkerHP");
                    break;
                case TechType.Warper:
                    ChangeHealth(__creature, "WarperHP");
                    break;
            }
        }

        public static void ChangeHealth(GameObject __instance, string __defaultHPValueKey)
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
                float __HPValueToAssign = CalculateHPToAssign( __defaultHPValueKey);

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
        }

        //Passed custom health value and default health value from dictionary; returns either the health value to assign, or -1 if there is no such key in the dictionary
        public static float CalculateHPToAssign(string __defaultHPValueKey)
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
        public static void MakeInvulnerable(GameObject __instance)
        {
            __instance.GetComponent<LiveMixin>().invincible = true;
        }
    }
}
