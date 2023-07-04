using HarmonyLib;
using System;
using System.IO;
using BepInEx.Logging;
using Nautilus.Handlers;
using static CreatureConfig.CreatureConfigPlugin;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using Nautilus.Json.Converters;

namespace CreatureConfig
{
    [HarmonyPatch]
    internal class CreatureConfig
    {
        static readonly Dictionary<string, float> defaultDamageValues = new Dictionary<string, float>()
        {
            { "AmpeelBiteDmg",30F },
            { "AmpeelShockDmg",15F },
            { "AmpeelCyclopsDmg",50F },
            { "BiterDmg",7F },
            { "BleederDmg",5F },
            { "BloodCrawlerDmg",5F },
            { "BlighterDmg",7F },
            { "BonesharkDmg",30F },
            { "CaveCrawlerDmg",5F },
            { "CrabsnakeDmg",35F },
            { "CrabsquidDmg",40F },
            { "CrashfishDmg",50F },
            { "GasopodGasPodDmg",10F },
            { "GhostLeviathanDmg",85F },
            { "GhostLeviathanCyclopsDmg",250F },
            { "GhostLeviathanJuvenileDmg",55F },
            { "GhostLeviathanJuvenileCyclopsDmg",220F },
            { "LavaLizardBiteDmg",30F },
            { "LavaLizardLavaRockDmg",15F },
            { "MesmerDmg",35F },
            { "ReaperDmg",80F },
            { "ReaperCyclopsDmg",220F },
            { "RiverProwlerDmg",30F },
            { "SandsharkDmg",30F },
            { "SeaDragonBiteDmg",300F },
            { "SeaDragonSwatDmg",70F },
            { "SeaDragonShoveDmg",250F },
            { "SeaDragonBurningChunkDmg",10F },
            { "SeaDragonLavaMeteorDmg",40F },
            { "SeaTreaderDmg",40F },
            { "StalkerDmg", 30F },
            { "WarperClawDmg", 30F },
            { "WarperWarpDmg", 10F }
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
                case TechType.Blighter:
                    ChangeGenericMeleeAttack(__instance, config.BlighterDmg, "BlighterDmg");
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
                case TechType.LavaLizard:
                    ChangeGenericMeleeAttack(__instance, config.LavaLizardBiteDmg, "LavaLizardBiteDmg");
                    GameObject __LL_projectile1 = __instance.gameObject.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    __LL_projectile1.GetComponent<LavaMeteor>().damage = config.LavaLizardLavaRockDmg;
                    break;
                case TechType.Mesmer:
                    ChangeGenericMeleeAttack(__instance, config.MesmerDmg, "MesmerDmg");
                    break;
                case TechType.SpineEel: //TechType for River Prowler
                    ChangeGenericMeleeAttack(__instance, config.RiverProwlerDmg, "RiverProwlerDmg");
                    break;
                case TechType.Sandshark:
                    ChangeGenericMeleeAttack(__instance, config.SandSharkDmg, "SandsharkDmg");
                    break;
                case TechType.Stalker:
                    ChangeGenericMeleeAttack(__instance, config.StalkerDmg, "StalkerDmg");
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage; change case by case)
                //This includes; Bleeder (Done), Crabsnake (Done), Ghost (done), Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Shocker: //TeechType for Ampeel
                    __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = config.AmpeelBiteDmg;
                    __instance.gameObject.GetComponent<ShockerMeleeAttack>().electricalDamage = config.AmpeelShockDmg;
                    __instance.gameObject.GetComponent<ShockerMeleeAttack>().cyclopsDamage = config.AmpeelCyclopsDmg;
                    break;
                case TechType.Bleeder:
                    logger.Log(LogLevel.Info, $"Found Bleeder; setting damage to {config.BleederDmg}");
                    __instance.gameObject.GetComponent<AttachAndSuck>().leechDamage = config.BleederDmg;
                    break;
                case TechType.Crabsnake: //Damage to player and damage to vehicles are seperate variables, but ulitmately the same damage value
                    logger.Log(LogLevel.Info, $"Found Crabsnake; setting damage to {config.CrabsnakeDmg}");
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().biteDamage = config.CrabsnakeDmg;
                    __instance.gameObject.GetComponent<CrabsnakeMeleeAttack>().seamothDamage = config.CrabsnakeDmg;
                    break;
                case TechType.Crash: //TechType for Crashfish
                    //Uses Publicizer
                    __instance.gameObject.GetComponent<Crash>().maxDamage = config.CrashfishDmg;
                    break;
                //case TechType.Gasopod:
                //NOTE!! The Gasopod only has a reference to the GasPod prefab; unlike the other projectile attacks, this doesn't state its own
                //instead it's just spawning GasPods based on the reference to the default GasPod prefab in the files, rather than stating its own custom version
                //As a result, we need to patch into the function used to spawn the gaspods to alter their damage
                //Patch is located below; "PrefixGasPod"
                //break;
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
                    //logger.Log(LogLevel.Info, $"Found Reaper Leviathan; setting player damage to {config.ReaperDmg} and cyclops damage to {config.ReaperCyclopsDmg}");
                    ChangeUniqueAttack(__instance, ref __instance.gameObject.GetComponent<ReaperMeleeAttack>().biteDamage, config.ReaperDmg, "ReaperDmg");
                    ChangeUniqueAttack(__instance, ref __instance.gameObject.GetComponent<ReaperMeleeAttack>().cyclopsDamage, config.ReaperCyclopsDmg, "ReaperCyclopsDmg");
                    //__instance.gameObject.GetComponent<ReaperMeleeAttack>().biteDamage = config.ReaperDmg; //80; Damage dealt to player, seamoth and prawn suit
                    //__instance.gameObject.GetComponent<ReaperMeleeAttack>().cyclopsDamage = config.ReaperCyclopsDmg; //220; Damage dealt to cyclops
                    //__instance.gameObject.GetComponent<ReaperMeleeAttack>().biteDamage = config.ReaperDmg; //80; Damage dealt to player, seamoth and prawn suit
                    //__instance.gameObject.GetComponent<ReaperMeleeAttack>().cyclopsDamage = config.ReaperCyclopsDmg; //220; Damage dealt to cyclops
                    break;
                case TechType.SeaDragon:
                    logger.Log(LogLevel.Info, $"Found Sea Dragon Leviathan; setting bite damage to {config.SeaDragonBiteDmg}, swat damage to {config.SeaDragonSwatDmg} and shove damage to {config.SeaDragonShoveDmg}");
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().biteDamage = config.SeaDragonBiteDmg; //300; Bite (so far for player and seamoth; untested on prawn suit)
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage = config.SeaDragonSwatDmg; //70; Swatted with arms (only for player, seamoth and prawn suit)
                    __instance.gameObject.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage = config.SeaDragonShoveDmg; //250; Shove when shoving into the cyclops
                    logger.Log(LogLevel.Info, $"For Sea Dragon Leviathan projectiles, setting burning chunk damage to {config.SeaDragonBurningChunkDmg} and lava meteor damage to {config.SeaDragonLavaMeteorDmg}");
                    GameObject __SD_projectile1 = __instance.gameObject.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    __SD_projectile1.GetComponent<BurningChunk>().fireDamage = config.SeaDragonBurningChunkDmg;
                    GameObject __SD_projectile2 = __instance.gameObject.GetComponent<RangedAttackLastTarget>().attackTypes[1].ammoPrefab;
                    __SD_projectile2.GetComponent<LavaMeteor>().damage = config.SeaDragonLavaMeteorDmg;

                    //??; Spawns fireballs; posisbly two types; 1 LavaMeteor or <=80 BurningChunks
                    // The LavaMeteor as a prefab has a default of 10 damage; 40 when spawned by the seadragon (as this is what it is in their ammoPrefab)...
                    // ...yet one-shot a seamoth and left the player on 20 health (80 damage from inside). However, a second attempt caused 60 damage to the Seamoth
                    // The BurningChunk as a prefab has fire damage of 5; 10 when spawned by seadragon (as this is what it is in their ammoPrefab)...
                    // ...yet appears to do no damage to a player, seamoth or cyclops (just a bunch of sounds akin to schools of fish hitting the screen)
                    break;
                case TechType.SeaTreader:
                    __instance.gameObject.GetComponent<SeaTreaderMeleeAttack>().damage = config.SeaTreaderDmg;
                    break;
                case TechType.Warper:
                    __instance.gameObject.GetComponent<WarperMeleeAttack>().biteDamage = config.WarperClawDmg;
                    GameObject __WP_projectile1 = __instance.gameObject.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    __WP_projectile1.GetComponent<WarpBall>().damage = config.WarperWarpDmg;
                    break;
            }
        }

        [HarmonyPatch(typeof(GasPod), nameof(GasPod.Start))]
        [HarmonyPostfix]
        public static void PrefixGasPod(GasPod __instance)
        {
            //NOTE!! The Gasopod only has a reference to the GasPod prefab; unlike the other projectile attacks, this doesn't state its own
            //instead it's just spawning GasPods based on the reference to the default GasPod prefab in the files, rather than stating its own custom version
            //As a result, we need to patch into the function used to spawn the gaspods to alter their damage
            //NOTE!! This will not change the damage of gaspods dropped by the player, meaning are still viable to use by the player
            logger.Log(LogLevel.Info, $"Found GasPod; setting damage to {config.GasopodGasPodDmg}");
            __instance.damagePerSecond = config.GasopodGasPodDmg;
        }

        public static void ChangeGenericMeleeAttack(Creature __instance, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; prints creature type
            TechType __techType = CraftData.GetTechType(__instance.gameObject);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if(__dmgValueToAssign != -1)
            {
                //DEBUG CODE; prints damage assigned
                logger.Log(LogLevel.Info, $"Setting {__techType} biteDamage to {__dmgValueToAssign}");

                //Set biteDamage to new damage value
                __instance.gameObject.GetComponent<MeleeAttack>().biteDamage = __dmgValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning damage
                logger.Log(LogLevel.Info, $"Failed setting {__techType} biteDamage");
            }
        }

        //public static void ChangeUniqueAttack<T>(Creature __instance, ref T uniqueAttackDmg, float __customDmgValue, string __defaultDmgValueKey)
        public static void ChangeUniqueAttack(Creature __instance, ref float __uniqueAttackDmg, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; prints creature type
            TechType __techType = CraftData.GetTechType(__instance.gameObject);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if (__dmgValueToAssign != -1)
            {
                //DEBUG CODE; prints damage assigned
                logger.Log(LogLevel.Info, $"Setting {__techType} {__defaultDmgValueKey} to {__dmgValueToAssign}");

                //Set unique attack damage to new damage value, by reference
                __uniqueAttackDmg = __dmgValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning damage
                logger.Log(LogLevel.Info, $"Failed setting {__techType} {__defaultDmgValueKey}");
            }
        }

        //Passed custom damage value and default damage value from dictionary; returns either the damage value to assign, or -1 if there is no such key in the dictionary
        //Both ChangeAttack methods use this code; reduces redundant code
        public static float CalculateDmgToAssign(float __customDmgValue, string __defaultDmgValueKey)
        {
            //First, check if key for default damage value actually exists in the dictionary
            if (defaultDamageValues.ContainsKey(__defaultDmgValueKey))
            {
                //If it exists, assign the default damage value to a variable
                float __defaultDmgValue = defaultDamageValues[__defaultDmgValueKey];

                //Store value to assign as new damage for the selected creature's unique attack
                //Default value is the default damage for the creature's unique attack
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

                //Return attack damage value to assign
                return __dmgValueToAssign;
            }
            else
            {
                //Return -1 if the calculation failed and the key did not exist in the default damage dictionary
                logger.Log(LogLevel.Error, $"Default Damage Value Key {__defaultDmgValueKey} does not exist in the dictionary!");
                return -1.0f;
            }
        }
    }
}
