﻿using HarmonyLib;
using BepInEx.Logging;
using static CreatureConfigDamage.CreatureConfigDamagePlugin;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using System;

namespace CreatureConfigDamage
{
    [HarmonyPatch]
    internal class CreatureConfigDamage
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
            { "DroopingStingerDmg", 50F },
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
            { "TigerPlantDmg", 10F },
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
                case TechType.Blighter:
                    ChangeGenericMeleeAttack(__creature, config.BlighterDmg, "BlighterDmg");
                    break;
                case TechType.Shuttlebug: //TechType for Blood Crawler
                    ChangeGenericMeleeAttack(__creature, config.BloodCrawlerDmg, "BloodCrawlerDmg");
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
                //This includes; Bleeder, Crabsnake, Ghost, Juvenile Emperor?, Reaper, Sea Dragon, Sea Trader, Ampeel (Shocker), Warper, Lava Lizard (ranged attack)
                case TechType.Shocker: //TechType for Ampeel
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
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().biteDamage, config.ReaperLeviathanDmg, "ReaperLeviathanDmg"); //80; Damage dealt to player, seamoth and prawn suit
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<ReaperMeleeAttack>().cyclopsDamage, config.ReaperLeviathanCyclopsDmg, "ReaperLeviathanCyclopsDmg"); //220; Damage dealt to cyclops
                    break;
                case TechType.SeaDragon:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().biteDamage, config.SeaDragonLeviathanBiteDmg, "SeaDragonBiteDmg"); //300; Bite (so far for player and seamoth; untested on prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage, config.SeaDragonLeviathanSwatDmg, "SeaDragonSwatDmg"); //70; Swatted with arms (only for player, seamoth and prawn suit)
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage, config.SeaDragonLeviathanShoveDmg, "SeaDragonShoveDmg"); //250; Shove when shoving into the cyclops
                    GameObject __SD_projectile1 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile1.GetComponent<BurningChunk>().fireDamage, config.SeaDragonLeviathanBurningChunkDmg, "SeaDragonBurningChunkDmg");
                    GameObject __SD_projectile2 = __creature.GetComponent<RangedAttackLastTarget>().attackTypes[1].ammoPrefab;
                    ChangeUniqueAttack(__creature, ref __SD_projectile2.GetComponent<LavaMeteor>().damage, config.SeaDragonLeviathanLavaMeteorDmg, "SeaDragonLavaMeteorDmg");
                    //NOTE!! Spawns fireballs; posisbly two types; 1 LavaMeteor or <=80 BurningChunks
                        // The LavaMeteor as a prefab has a default of 10 damage; 40 when spawned by the seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet one-shot a seamoth and left the player on 20 health (80 damage from inside). However, a second attempt caused 60 damage to the Seamoth
                        // The BurningChunk as a prefab has fire damage of 5; 10 when spawned by seadragon (as this is what it is in their ammoPrefab)...
                        // ...yet appears to do no damage to a player, seamoth or cyclops (just a bunch of sounds akin to schools of fish hitting the screen)
                    break;
                case TechType.SeaTreader:
                    ChangeUniqueAttack(__creature, ref __creature.GetComponent<SeaTreaderMeleeAttack>().damage, config.SeaTreaderLeviathanDmg, "SeaTreaderDmg");
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
        public static void PostfixGasPod(GasPod __instance)
        {
            //NOTE!! The Gasopod only has a reference to the GasPod prefab; unlike the other projectile attacks, this doesn't state its own
                //instead it's just spawning GasPods based on the reference to the default GasPod prefab in the files, rather than stating its own custom version
                //As a result, we need to patch into the function used to spawn the gaspods to alter their damage
            ChangeUniqueAttack(__instance.gameObject, ref __instance.damagePerSecond, config.GasopodGasPodDmg, "GasopodGasPodDmg");
        }

        /* 80 - (80*0.52) = 38.4
         * 80/38.4 = 2.083333333 = 1/0.48
         * 80 - (80*0.4) = 48
         * 80/48 = 1.666666666 = 1/0.6
         * 80 - (80*0.12) = 70.4
         * 80/70.4 = 1.136363636 = 1/0.88
         * modifier = 1/(1-num2) where num2 is the % protection reinforced dive suit pieces give
         */

        //Patch damage calculations, "ignoring" the reduction in damage from the reinforced diving suit, if a creature has been set by the user to ignore it.
        //NOTE!! Could instead of much of this, just PreFix and check if the creature ignores damage, and the player has armour, then make the damage 'DamageType.Starve', which ignores the armour by default
        //Only issue there would be it would remove some damage FX if it changes the damage type away from, say, Normal or Fire
        //TODO!! Say, for example, a reaper does 80 damage. It plays an animation when it kills the player. With the armour, it deals ~38 damage. If we set it to ignore, and it kills the player, it *DOES NOT* play the kill animation
        [HarmonyPatch(typeof(DamageSystem), nameof(DamageSystem.CalculateDamage))]
        [HarmonyPostfix]
        //TODO!! IF CAN'T FIGURE OUT THE NULL DEALER CAUSING MISSING DEATH GRAB ANIMATIONS, JUST IGNORE IT AND RELEASE IT!!
        public static float PostfixCalculateDamage(float damage, DamageType type, GameObject target, GameObject dealer)
        {
            logger.LogError($"damage is {damage}");
            logger.LogError($"type is {type}");
            logger.LogError($"target is {target}");
            //logger.LogError($"dealer is {dealer}");
            if (dealer != null) 
            {
                logger.LogError($"dealer is {dealer}");
                bool playerTarget = target.GetComponent<Player>();

                if (playerTarget && type != DamageType.Radiation && type != DamageType.Starve)
                {
                    //Recalculate the armour value of any reinforced suit pieces and how much they've reduced the incoming damage
                    float armourValue = 0f;
                    if (Player.main.HasReinforcedSuit())
                    {
                        armourValue += 0.4f;
                    }
                    if (Player.main.HasReinforcedGloves())
                    {
                        armourValue += 0.12f;
                    }

                    TechType dealerTechType = CraftData.GetTechType(dealer);

                    if (armourValue > 0 && config.IgnoreArmour.ContainsKey(dealerTechType))
                    {
                        if (config.IgnoreArmour[dealerTechType])
                        {
                            logger.LogError($"{dealerTechType} is ignoring armour.");
                            logger.LogInfo($"Reduced Damage = {damage}");

                            //Calculate what to multiply the reduced damage by to restore it back to full damage
                            float originalDamageMultiplier = 1 / (1 - armourValue);

                            logger.LogInfo($"Original Damage = {damage * originalDamageMultiplier}");
                            damage *= originalDamageMultiplier;
                            logger.LogInfo($"Final to Player = {damage}");
                            return damage;
                        }
                        else
                        {
                            logger.LogError($"{dealerTechType} is not ignoring armour.");
                            logger.LogInfo($"Final Damage to Player = {damage}");
                        }
                    }
                }
            }
            
            return damage; //Returns the damage value calculated at the end of the patched function, not the damage given at the start
        }

        #region DEBUG!! An attempt to patch entirely overwrite the ReaperMeleeAttack.OnTouch, *just* to change the dealer from null to reaper
        //ATTEMPT 1
        /*[HarmonyTranspiler]
        [HarmonyDebug]
        [HarmonyPatch(typeof(ReaperMeleeAttack), nameof(ReaperMeleeAttack.OnTouch))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //ldnull means null in IL; that's what we're looking to replace
            //ERROR!! There are TWO nulls in the code; I need to check for the SECOND one, which I'm NOT doing yet!
            CodeMatch nullMatch = new CodeMatch(i => i.opcode == OpCodes.Ldnull);

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, nullMatch)
                .ThrowIfInvalid("Invalid")
                .ThrowIfNotMatch("Not a match")
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<ReaperMeleeAttack, GameObject>>(MyFunctionIWrote));

            //stsfld <field>	Replace the value of the static field with val.

            foreach (var item in newInstructions.InstructionEnumeration())
            {
                //logger.LogError($"{item.opcode} {item.operand}");
            }

            return newInstructions.InstructionEnumeration();
        }*/

        //ATTEMPT 2
        [HarmonyTranspiler]
        [HarmonyDebug]
        [HarmonyPatch(typeof(ReaperMeleeAttack), nameof(ReaperMeleeAttack.OnTouch))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            int insertionIndex = -1;
            int GoForSecondNull = 0;
            for (int i = 0; i < code.Count - 1; i++) // -1 since we will be checking i + 1
            {
                if (code[i].opcode == OpCodes.Ldnull && code[i + 1].opcode == OpCodes.Ret)
                {
                    if(GoForSecondNull == 1)
                    {
                        insertionIndex = i;
                        break;
                    }
                    GoForSecondNull++; //So it goes for the second instance of null, not the first
                    //This probably doesn't work...
                }
            }

            return code;
        }

        public static GameObject MyFunctionIWrote(ReaperMeleeAttack attack)
        {
            GameObject reaper = attack.reaper.gameObject;

            logger.LogError("Transpiler function running");
            logger.LogError($"Transpiler found reaper {attack} = {reaper}");
            logger.LogError($"Found reaper is of size = {reaper.transform.localScale.x}");

            return reaper;
        }

        /*[HarmonyPatch(typeof(ReaperMeleeAttack), nameof(ReaperMeleeAttack.OnTouch))]
        [HarmonyPrefix]
        public static void OnTouch(Collider collider, ReaperMeleeAttack __instance)
        {
            if (__instance.liveMixin.IsAlive() && Time.time > __instance.timeLastBite + __instance.biteInterval && __instance.reaper.Aggression.Value >= 0.5f)
            {
                GameObject target = __instance.GetTarget(collider);
                if (!__instance.reaper.IsHoldingVehicle() && !__instance.playerDeathCinematic.IsCinematicModeActive())
                {
                    Player component = target.GetComponent<Player>();
                    if (component != null)
                    {
                        if (component.CanBeAttacked() && !component.cinematicModeActive)
                        {
                            float num = DamageSystem.CalculateDamage(__instance.biteDamage, DamageType.Normal, component.gameObject, __instance.reaper.gameObject);
                            if (component.GetComponent<LiveMixin>().health - num <= 0f)
                            {
                                __instance.playerDeathCinematic.StartCinematicMode(component);
                                if (__instance.playerAttackSound)
                                {
                                    __instance.playerAttackSound.Play();
                                }
                                __instance.reaper.OnGrabPlayer();
                            }
                        }
                    }
                    else if (__instance.reaper.GetCanGrabVehicle())
                    {
                        SeaMoth component2 = target.GetComponent<SeaMoth>();
                        if (component2 && !component2.docked)
                        {
                            __instance.reaper.GrabSeamoth(component2);
                        }
                        Exosuit component3 = target.GetComponent<Exosuit>();
                        if (component3 && !component3.docked)
                        {
                            __instance.reaper.GrabExosuit(component3);
                        }
                    }
                    base.OnTouch(collider);
                    __instance.reaper.Aggression.Value -= 0.25f;
                }
            }
        }*/
        #endregion

        /*[HarmonyPatch(typeof(HangingStinger), nameof(HangingStinger.OnCollisionEnter))]
        [HarmonyPrefix]
        public static void PrefixHangingStingerCollision(HangingStinger __instance)
        {
	        //NOTE!! As Hanging Stinger is considered Flora, we can't patch into it like we did the creatures; it's only one of two damaging plants anyway

	        if (__instance._venomAmount >= 1f && other.gameObject.GetComponentInChildren<LiveMixin>() != null)
	        {
		        DamageOverTime damageOverTime = other.gameObject.AddComponent<DamageOverTime>();
		        damageOverTime.doer = base.gameObject;
		        damageOverTime.totalDamage = 30f;
		        damageOverTime.duration = 2.5f * (float)__instance.size;
		        damageOverTime.damageType = DamageType.Poison;
		        damageOverTime.ActivateInterval(0.5f);
		        __instance._venomAmount = 0f;
		        __instance.venomRechargeTime = UnityEngine.Random.value * 5f + 5f;
	        }
        }*/

        /*[HarmonyPatch(typeof(SpikePlant), nameof(SpikePlant.Start))]
        [HarmonyPrefix]
        public static void PrefixTigerPlant(SpikePlant __instance)
        {
            //NOTE!! As Tiger Plant is considered Flora, we can't patch into it exactly like we did the creatures; it's only one of two damaging plants anyway
            //However, unlike the Drooping Stinger, the Tiger Plant has a component with a damage value we can change relatively easily
            logger.Log(LogLevel.Error, "Tiger Plant Found");
            ChangeUniqueAttack(__instance.gameObject, ref __instance.GetComponent<RangeAttacker>().damage, config.TigerPlantDmg, "TigerPlantDmg");
        }*/

        public static void ChangeGenericMeleeAttack(GameObject __instance, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

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

        public static void ChangeUniqueAttack(GameObject __instance, ref float __uniqueAttackDmg, float __customDmgValue, string __defaultDmgValueKey)
        {
            //DEBUG CODE; retrieves creature type
            TechType __techType = CraftData.GetTechType(__instance);

            //Calculate correct damage value to assign to creature's generic melee attack; returns -1 if no such attack exists in the dictionary
            float __dmgValueToAssign = CalculateDmgToAssign(__customDmgValue, __defaultDmgValueKey);

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
