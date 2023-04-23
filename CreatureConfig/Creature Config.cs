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
        static readonly Dictionary<string, float> defaultDamageValues = new Dictionary<string, float>()
        {
            { "BiterDmg",7F },
            { "BleederDmg",5F },
            { "BloodCrawlerDmg",5F },
            { "BonesharkDmg",30F },
            { "CaveCrawlerDmg",5F },
            { "CrabsnakeDmg",35F },
            { "CrabsquidDmg",40F },
            { "GhostLeviathanDmg",85F },
            { "GhostLeviathanCyclopsDmg",250F },
            { "GhostLeviathanJuvenileDmg",55F },
            { "GhostLeviathanJuvenileCyclopsDmg",220F },
            { "LavaLizardBiteDmg",30F },
            { "LavaLizardSpitDmg",30F },
            { "MesmerDmg",35F },
            { "ReaperDmg",80F },
            { "ReaperCyclopsDmg",220F },
            { "SandsharkDmg",30F },
            { "SeaDragonBiteDmg",300F },
            { "SeaDragonSwatDmg",70F },
            { "SeaDragonShoveDmg",250F },
            { "StalkerDmg", 30F }
        };

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
        [HarmonyPrefix]
        public static void PrefixCreatureStart(Creature __instance)
        {
            TechType __techType = CraftData.GetTechType(__instance.gameObject);
            
            switch (__techType)
            {
                //Handle generic cases (just a MeleeAttack component; change biteDamage)
                case TechType.Biter:
                    ChangeGenericMeleeAttack(__instance, config.BiterDmg, "BiterDmg");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeGenericMeleeAttack(__instance, config.BloodCrawlerDmg, "BloodCrawlerDmg");
                    break;
                case TechType.BoneShark:
                    ChangeGenericMeleeAttack(__instance, config.BonesharkDmg, "BonesharkDmg");
                    break;
                case TechType.CaveCrawler:
                    ChangeGenericMeleeAttack(__instance, config.CaveCrawlerDmg, "CaveCrawlerDmg");
                    break;
                case TechType.CrabSquid:
                    ChangeGenericMeleeAttack(__instance, config.CrabsquidDmg, "CrabsquidDmg");
                    break;
                case TechType.LavaLizard: //Ranged attack is handled in unique cases
                    ChangeGenericMeleeAttack(__instance, config.LavaLizardBiteDmg, "LavaLizardBiteDmg");
                    break;
                case TechType.Mesmer:
                    ChangeGenericMeleeAttack(__instance, config.MesmerDmg, "MesmerDmg");
                    break;
                case TechType.Sandshark:
                    ChangeGenericMeleeAttack(__instance, config.SandSharkDmg, "SandsharkDmg");
                    break;
                case TechType.Stalker:
                    ChangeGenericMeleeAttack(__instance, config.StalkerDmg, "StalkerDmg");
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage; change case by case)
                //This includes; Bleeder (Done), Crabsnake (Done), Ghost (done), Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Bleeder:
                    logger.Log(LogLevel.Info, $"Found Bleeder; setting damage to {config.BleederDmg}");
                    __instance.gameObject.GetComponent<AttachAndSuck>().leechDamage = config.BleederDmg;
                    break;
                case TechType.Crabsnake: //Damage to player and damage to vehicles are seperate variables, but ulitmately the same damage value
                    logger.Log(LogLevel.Info, $"Found Crabsnake; setting damage to {config.CrabsnakeDmg}");
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().biteDamage = config.CrabsnakeDmg;
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().seamothDamage = config.CrabsnakeDmg;
                    break;
                case TechType.GhostLeviathan:
                    logger.Log(LogLevel.Info, $"Found Ghost Leviathan; setting player damage to {config.GhostLeviathanDmg} and cyclops damage to {config.GhostLeviathanCyclopsDmg}");
                    __instance.gameObject.GetComponent<GhostLeviathanMeleeAttack>().biteDamage = config.GhostLeviathanDmg; //85; Damage dealt to player, seamoth and prawn suit
                    __instance.gameObject.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage = config.GhostLeviathanCyclopsDmg; //250; Damage dealt to cyclops
                    break;
                case TechType.GhostLeviathanJuvenile:
                    logger.Log(LogLevel.Info, $"Found Ghost Leviathan Juvenile; setting player damage to {config.GhostLeviathanJuvenileDmg} and cyclops damage to {config.GhostLeviathanJuvenileCyclopsDmg}");
                    __instance.gameObject.GetComponent<GhostLeviathanMeleeAttack>().biteDamage = config.GhostLeviathanJuvenileDmg; //55; Damage dealt to player, seamoth and prawn suit
                    __instance.gameObject.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage = config.GhostLeviathanJuvenileCyclopsDmg; //220; Damage dealt to cyclops
                    break;
                case TechType.ReaperLeviathan:
                    logger.Log(LogLevel.Info, $"Found Reaper Leviathan; setting player damage to {config.ReaperDmg} and cyclops damage to {config.ReaperCyclopsDmg}");
                    __instance.gameObject.GetComponent<ReaperMeleeAttack>().biteDamage = config.ReaperDmg; //80; Damage dealt to player, seamoth and prawn suit
                    __instance.gameObject.GetComponent<ReaperMeleeAttack>().cyclopsDamage = config.ReaperCyclopsDmg; //220; Damage dealt to cyclops
                    break;
                case TechType.SeaDragon:
                    logger.Log(LogLevel.Info, $"Found Sea Dragon Leviathan; setting bite damage to {config.ReaperDmg} and swat damage to {config.ReaperCyclopsDmg}");
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().biteDamage = config.SeaDragonBiteDmg; //300; Bite (at least for player; unclear if seamoth or prawn suit are affected; might only be swat for them)
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage = config.SeaDragonSwatDmg; //70; Swatted with arms (only for player, seamoth and prawn suit)
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage = config.SeaDragonShoveDmg; //250; Shove when shoving into the cyclops
                    break;
            }
        }

        public static void ChangeGenericMeleeAttack(Creature __instance, float __customDmgValue, string __defaultDmgValueKey)
        {
            //First, check if key for default damage value actually exists in the dictionary
            if (defaultDamageValues.ContainsKey(__defaultDmgValueKey))
            {
                //If it exists, assign the default damage value to a variable
                float __defaultDmgValue = defaultDamageValues[__defaultDmgValueKey];

                //Store value to assign as new biteDamage for the selected creature; default value is the default biteDamge for the creature
                float __dmgValueToAssign = __defaultDmgValue;

                //Obtain preset and determine which damage value to assign according to the preset
                float __preset = config.DamagePreset;
                logger.Log(LogLevel.Info, $"Preset = {__preset}");

                switch (__preset)
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
                        __dmgValueToAssign = (__preset - 1) / 4 * __defaultDmgValue;
                        break;
                }

                //DEBUG CODE; prints creature type and damage assigned
                TechType __techType = CraftData.GetTechType(__instance.gameObject);
                logger.Log(LogLevel.Info, $"Setting {__techType} biteDamage to {__dmgValueToAssign}");

                //Set biteDamage to new damage value
                __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = __dmgValueToAssign;
            }
            else
            {
                logger.Log(LogLevel.Error, $"Default Damage Value Key {__defaultDmgValueKey} does not exist in the dictionary!");
            }
        }
    }
}
