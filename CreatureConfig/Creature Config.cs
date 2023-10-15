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
            //Reference the gameObject Class directly, as none of the functionality uses the Creature class specifically
            GameObject __creature = __instance.gameObject;

            TechType __techType = CraftData.GetTechType(__creature);
            
            switch (__techType)
            {
                //Handle generic cases (just a MeleeAttack component; change biteDamage)
                case TechType.Biter:
                    ChangeGenericMeleeAttack(__creature, config.BiterDmg, "BiterDmg");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeGenericMeleeAttack(__creature, config.BloodCrawlerDmg, "BloodCrawlerDmg");
                    break;
                case TechType.Blighter:
                    ChangeGenericMeleeAttack(__creature, config.BlighterDmg, "BlighterDmg");
                    break;
                case TechType.BoneShark:
                    ChangeGenericMeleeAttack(__creature, config.BonesharkDmg, "BonesharkDmg");
                    break;
                case TechType.CaveCrawler:
                    ChangeGenericMeleeAttack(__creature, config.CaveCrawlerDmg, "CaveCrawlerDmg");
                    break;
                case TechType.CrabSquid:
                    ChangeGenericMeleeAttack(__creature, config.CrabsquidDmg, "CrabsquidDmg");
                    break;
                case TechType.Mesmer:
                    ChangeGenericMeleeAttack(__creature, config.MesmerDmg, "MesmerDmg");
                    break;
                case TechType.SpineEel: //TechType for River Prowler
                    ChangeGenericMeleeAttack(__creature, config.RiverProwlerDmg, "RiverProwlerDmg");
                    break;
                case TechType.Sandshark:
                    ChangeGenericMeleeAttack(__creature, config.SandSharkDmg, "SandsharkDmg");
                    break;
                case TechType.Stalker:
                    ChangeGenericMeleeAttack(__creature, config.StalkerDmg, "StalkerDmg");
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage, or shoots a projectile; change case by case)
                //This includes; Bleeder (Done), Crabsnake (Done), Ghost (done), Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Shocker: //TeechType for Ampeel
                    ChangeGenericMeleeAttack(__creature, config.AmpeelBiteDmg, "AmpeelBiteDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ShockerMeleeAttack>().electricalDamage, config.AmpeelShockDmg, "AmpeelShockDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ShockerMeleeAttack>().cyclopsDamage, config.AmpeelCyclopsDmg, "AmpeelCyclopsDmg");
                    break;
                case TechType.Bleeder:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<AttachAndSuck>().leechDamage, config.BleederDmg, "BleederDmg");
                    break;
                case TechType.Crabsnake: //Damage to player and damage to vehicles are seperate variables, but ulitmately the same damage value
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<CrabsnakeMeleeAttack>().biteDamage, config.CrabsnakeDmg, "CrabsnakeDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<CrabsnakeMeleeAttack>().seamothDamage, config.CrabsnakeDmg, "CrabsnakeDmg");
                    break;
                case TechType.Crash: //TechType for Crashfish
                    //Private field; Uses Publicizer to access
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<Crash>().maxDamage, config.CrashfishDmg, "CrashfishDmg");
                    break;
                case TechType.LavaLizard:
                    ChangeGenericMeleeAttack(__creature, config.LavaLizardBiteDmg, "LavaLizardBiteDmg");
                    GameObject __LL_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __LL_projectile1.GetComponent<LavaMeteor>().damage, config.LavaLizardLavaRockDmg, "LavaLizardLavaRockDmg");
                    break;
                case TechType.GhostLeviathan:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, config.GhostLeviathanDmg, "GhostLeviathanDmg"); //85; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, config.GhostLeviathanCyclopsDmg, "GhostLeviathanCyclopsDmg"); //250; Damage dealt to cyclops
                    break;
                case TechType.GhostLeviathanJuvenile:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, config.GhostLeviathanJuvenileDmg, "GhostLeviathanJuvenileDmg"); //55; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, config.GhostLeviathanJuvenileCyclopsDmg, "GhostLeviathanJuvenileCyclopsDmg"); //220; Damage dealt to cyclops
                    break;
                case TechType.ReaperLeviathan:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().biteDamage, config.ReaperDmg, "ReaperDmg"); //80; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().cyclopsDamage, config.ReaperCyclopsDmg, "ReaperCyclopsDmg"); //220; Damage dealt to cyclops
                    break;
                case TechType.SeaDragon:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().biteDamage, config.SeaDragonBiteDmg, "SeaDragonBiteDmg"); //300; Bite (so far for player and seamoth; untested on prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage, config.SeaDragonSwatDmg, "SeaDragonSwatDmg"); //70; Swatted with arms (only for player, seamoth and prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage, config.SeaDragonShoveDmg, "SeaDragonShoveDmg"); //250; Shove when shoving into the cyclops
                    GameObject __SD_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile1.GetComponent<BurningChunk>().fireDamage, config.SeaDragonBurningChunkDmg, "SeaDragonBurningChunkDmg");
                    GameObject __SD_projectile2 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[1].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile2.GetComponent<LavaMeteor>().damage, config.SeaDragonLavaMeteorDmg, "SeaDragonLavaMeteorDmg");
                    //NOTE!! Spawns fireballs; posisbly two types; 1 LavaMeteor or <=80 BurningChunks
                        // The LavaMeteor as a prefab has a default of 10 damage; 40 when spawned by the seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet one-shot a seamoth and left the player on 20 health (80 damage from inside). However, a second attempt caused 60 damage to the Seamoth
                        // The BurningChunk as a prefab has fire damage of 5; 10 when spawned by seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet appears to do no damage to a player, seamoth or cyclops (just a bunch of sounds akin to schools of fish hitting the screen)
                    break;
                case TechType.SeaTreader:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaTreaderMeleeAttack>().damage, config.SeaTreaderDmg, "SeaTreaderDmg");
                    break;
                case TechType.Warper:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<WarperMeleeAttack>().biteDamage, config.WarperClawDmg, "WarperClawDmg");
                    GameObject __WP_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __WP_projectile1.GetComponent<WarpBall>().damage, config.WarperWarpDmg, "WarperWarpDmg");
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
            ChangeUniqueAttack(__instance.gameObject, ref __instance.damagePerSecond, config.GasopodGasPodDmg, "GasopodGasPodDmg");
        }

        public static void ChangeGenericMeleeAttack(GameObject __instance, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if(__dmgValueToAssign != -1)
            {
                //DEBUG CODE; prints creature type and damage assigned
                logger.Log(LogLevel.Info, $"Setting {__techType} biteDamage to {__dmgValueToAssign}");

                //Set biteDamage to new damage value
                __instance.GetComponent<MeleeAttack>().biteDamage = __dmgValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning damage
                logger.Log(LogLevel.Error, $"Failed setting {__techType} biteDamage");
            }
        }

        //public static void ChangeUniqueAttack<T>(Creature __instance, ref T uniqueAttackDmg, float __customDmgValue, string __defaultDmgValueKey)
        public static void ChangeUniqueAttack(GameObject __instance, ref float __uniqueAttackDmg, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if (__dmgValueToAssign != -1)
            {
                //DEBUG CODE; prints creature type and damage assigned
                logger.Log(LogLevel.Info, $"Setting {__techType} {__defaultDmgValueKey} to {__dmgValueToAssign}");

                //Set unique attack damage to new damage value, by reference
                __uniqueAttackDmg = __dmgValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning damage
                logger.Log(LogLevel.Error, $"Failed setting {__techType} {__defaultDmgValueKey}");
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
                //logger.Log(LogLevel.Info, $"Preset = {__preset}");

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

                    //Damage Presets 3,4,5,6,7, multiply default damage values by a percentage, based on the preset selected
                    //5 is Default, damage value is reset to default
                    case float n when n >= 3 && n <= 7:
                        __dmgValueToAssign = (__preset - 1) / 4 * __defaultDmgValue;
                        break;

                    //Sudden Death, make all damage values 1000
                    case 8:
                        __dmgValueToAssign = 1000;
                        break;

                    default:
                        logger.Log(LogLevel.Error, $"Preset {__preset} not recognised!");
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
