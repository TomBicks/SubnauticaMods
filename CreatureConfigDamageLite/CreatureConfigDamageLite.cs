using HarmonyLib;
using BepInEx.Logging;
using static CreatureConfigDamageLite.CreatureConfigDamageLitePlugin;
using System.Collections.Generic;
using UnityEngine;

namespace CreatureConfigDamageLite
{
    [HarmonyPatch]
    internal class CreatureConfigDamageLite
    {
        static readonly Dictionary<string, float> defaultDamageValues = new Dictionary<string, float>()
        {
            { "AmpeelBiteDmg",30F },
            { "AmpeelShockDmg",15F },
            { "AmpeelCyclopsDmg",50F },
            { "BiterDmg",7F },
            { "BleederDmg",5F },
            { "BlighterDmg",7F },
            { "BloodCrawlerDmg",5F },
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
            { "ReaperLeviathanDmg",80F },
            { "ReaperLeviathanCyclopsDmg",220F },
            { "RiverProwlerDmg",30F },
            { "SandsharkDmg",30F },
            { "SeaDragonLeviathanBiteDmg",300F },
            { "SeaDragonLeviathanSwatDmg",70F },
            { "SeaDragonLeviathanShoveDmg",250F },
            { "SeaDragonLeviathanBurningChunkDmg",10F },
            { "SeaDragonLeviathanLavaMeteorDmg",40F },
            { "SeaTreaderLeviathanDmg",40F },
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
                    ChangeGenericMeleeAttack(__creature, "BiterDmg");
                    break;
                case TechType.Blighter:
                    ChangeGenericMeleeAttack(__creature, "BlighterDmg");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeGenericMeleeAttack(__creature, "BloodCrawlerDmg");
                    break;
                case TechType.BoneShark:
                    ChangeGenericMeleeAttack(__creature, "BonesharkDmg");
                    break;
                case TechType.CaveCrawler:
                    ChangeGenericMeleeAttack(__creature, "CaveCrawlerDmg");
                    break;
                case TechType.CrabSquid:
                    ChangeGenericMeleeAttack(__creature, "CrabsquidDmg");
                    break;
                case TechType.Mesmer:
                    ChangeGenericMeleeAttack(__creature, "MesmerDmg");
                    break;
                case TechType.SpineEel: //TechType for River Prowler
                    ChangeGenericMeleeAttack(__creature, "RiverProwlerDmg");
                    break;
                case TechType.Sandshark:
                    ChangeGenericMeleeAttack(__creature, "SandsharkDmg");
                    break;
                case TechType.Stalker:
                    ChangeGenericMeleeAttack(__creature, "StalkerDmg");
                    break;

                //Handle unique cases (has a unique MeleeAttack component, often with a grab animation and cinematic damage, or shoots a projectile; change case by case)
                //This includes; Bleeder, Crabsnake, Ghost, Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Shocker: //TechType for Ampeel
                    ChangeGenericMeleeAttack(__creature, "AmpeelBiteDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ShockerMeleeAttack>().electricalDamage, "AmpeelShockDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ShockerMeleeAttack>().cyclopsDamage, "AmpeelCyclopsDmg");
                    break;
                case TechType.Bleeder:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<AttachAndSuck>().leechDamage, "BleederDmg");
                    break;
                case TechType.Crabsnake: //Damage to player and damage to vehicles are seperate variables, but ulitmately the same damage value
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<CrabsnakeMeleeAttack>().biteDamage, "CrabsnakeDmg");
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<CrabsnakeMeleeAttack>().seamothDamage, "CrabsnakeDmg");
                    break;
                case TechType.Crash: //TechType for Crashfish
                    //Private field; Uses Publicizer to access
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<Crash>().maxDamage, "CrashfishDmg");
                    break;
                case TechType.LavaLizard:
                    ChangeGenericMeleeAttack(__creature, "LavaLizardBiteDmg");
                    GameObject __LL_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __LL_projectile1.GetComponent<LavaMeteor>().damage, "LavaLizardLavaRockDmg");
                    break;
                case TechType.GhostLeviathan:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, "GhostLeviathanDmg"); //85; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, "GhostLeviathanCyclopsDmg"); //250; Damage dealt to cyclops
                    break;
                case TechType.GhostLeviathanJuvenile:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, "GhostLeviathanJuvenileDmg"); //55; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, "GhostLeviathanJuvenileCyclopsDmg"); //220; Damage dealt to cyclops
                    break;
                case TechType.ReaperLeviathan:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().biteDamage, "ReaperLeviathanDmg"); //80; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().cyclopsDamage, "ReaperLeviathanCyclopsDmg"); //220; Damage dealt to cyclops
                    break;
                case TechType.SeaDragon:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().biteDamage, "SeaDragonLeviathanBiteDmg"); //300; Bite (so far for player and seamoth; untested on prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage, "SeaDragonLeviathanSwatDmg"); //70; Swatted with arms (only for player, seamoth and prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage, "SeaDragonLeviathanShoveDmg"); //250; Shove when shoving into the cyclops
                    GameObject __SD_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile1.GetComponent<BurningChunk>().fireDamage, "SeaDragonLeviathanBurningChunkDmg");
                    GameObject __SD_projectile2 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[1].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile2.GetComponent<LavaMeteor>().damage, "SeaDragonLeviathanLavaMeteorDmg");
                    //NOTE!! Spawns fireballs; posisbly two types; 1 LavaMeteor or <=80 BurningChunks
                        // The LavaMeteor as a prefab has a default of 10 damage; 40 when spawned by the seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet one-shot a seamoth and left the player on 20 health (80 damage from inside). However, a second attempt caused 60 damage to the Seamoth
                        // The BurningChunk as a prefab has fire damage of 5; 10 when spawned by seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet appears to do no damage to a player, seamoth or cyclops (just a bunch of sounds akin to schools of fish hitting the screen)
                    break;
                case TechType.SeaTreader:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaTreaderMeleeAttack>().damage, "SeaTreaderLeviathanDmg");
                    break;
                case TechType.Warper:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<WarperMeleeAttack>().biteDamage, "WarperClawDmg");
                    GameObject __WP_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __WP_projectile1.GetComponent<WarpBall>().damage, "WarperWarpDmg");
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
            ChangeUniqueAttack(__instance.gameObject, ref __instance.damagePerSecond, "GasopodGasPodDmg");
        }

        public static void ChangeGenericMeleeAttack(GameObject __instance, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if(__dmgValueToAssign != -1)
            {
                //Set biteDamage to new damage value
                __instance.GetComponent<MeleeAttack>().biteDamage = __dmgValueToAssign;
            }
            else
            {
                //DEBUG CODE; prints failure in assigning damage
                logger.Log(LogLevel.Error, $"Failed setting {__techType} biteDamage");
            }
        }

        public static void ChangeUniqueAttack(GameObject __instance, ref float __uniqueAttackDmg, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__defaultDmgValueKey);

            //Check if the method managed to calculate a damage value to assign; -1 if it did not
            if (__dmgValueToAssign != -1)
            {
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
        public static float CalculateDmgToAssign(string __defaultDmgValueKey)
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

                switch (__preset)
                {
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
