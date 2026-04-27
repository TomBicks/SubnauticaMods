using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace CreatureConfigDamage;

internal static class Patches
{
    [HarmonyPatch(typeof(DamageSystem))]
    internal static class DamageSystemPatch
    {
        //Patch damage calculations, "ignoring" the reduction in damage from the reinforced diving suit, if a creature has been set by the user to ignore it.
        [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
        [HarmonyPostfix]
        public static float CalculateArmourDamage(float damage, DamageType type, GameObject target, GameObject dealer)
        {
            Plugin.Logger.LogError($"damage={damage}, type={type}, target={target}, dealer={dealer}");
            if (dealer != null)
            {
                bool playerTarget = target.GetComponent<Player>();

                if (playerTarget && type != DamageType.Radiation && type != DamageType.Starve)
                {
                    //Recalculate the armour value of any reinforced suit pieces (and therefore how much they'll reduce the incoming damage)
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

                    //If the player is wearing armour, and the creature is set to ignore it, remove the damage reduction
                    if (armourValue > 0 && Plugin.config.IgnoreArmour.TryGetValue(dealerTechType, out bool ignoring))
                    {
                        if (ignoring) //false if fails to find an entry, or if not ignoring armour; works either way
                        {
                            //Calculate what to multiply the reduced damage by to restore it back to full damage
                            float originalDamageMultiplier = 1 / (1 - armourValue);
                            return damage * originalDamageMultiplier;
                        }
                    }
                }
            }

            return damage; //Returns the damage value calculated at the end of the patched function, not the damage given at the start
        }

        //CALCULATIONS FROM TESTING
        /* 80 - (80*0.52) = 38.4
         * 80/38.4 = 2.083333333 = 1/0.48
         * 80 - (80*0.4) = 48
         * 80/48 = 1.666666666 = 1/0.6
         * 80 - (80*0.12) = 70.4
         * 80/70.4 = 1.136363636 = 1/0.88
         * modifier = 1/(1-num2) where num2 is the % protection reinforced dive suit pieces give
         */
    }

    [HarmonyPatch(typeof(ReaperMeleeAttack), nameof(ReaperMeleeAttack.OnTouch))]
    internal static class ReaperPatch
    {
        //Transpile the ReaperMeleeAttack OnTouch method, replacing the null dealer value CalculateAttack function passes to instead be a reference to the Reaper itself
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new CodeMatch(OpCodes.Ldnull))
                .ThrowIfInvalid("Reaper Transpiler Invalid")
                .ThrowIfNotMatch("Reaper Transpiler Not A Match")
                .Advance(1) //Move forward 1 index, so that we replace ldnull, and not the callvirt before it
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Set 'this' to top of stack, so next function takes that as input
                .Insert(Transpilers.EmitDelegate(GetReaperReference)); //Take 'this' as input and thus return reference to the reaper this attack belongs to

            return matcher.InstructionEnumeration();
        }

        //Return a reference to the Reaper the ReaperMeleeAttack component belongs to
        public static GameObject GetReaperReference(ReaperMeleeAttack attack)
        {
            GameObject reaper = attack.reaper.gameObject;

            return reaper;
        }
    }

    [HarmonyPatch(typeof(SeaDragonMeleeAttack), nameof(SeaDragonMeleeAttack.OnTouchFront))]
    internal static class SeaDragonPatch
    {
        //Transpile the SeaDragonMeleeAttack OnTouchFront method, replacing the null dealer value CalculateAttack function passes to instead be a reference to the Sea Dragon itself
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new CodeMatch(OpCodes.Ldnull))
                .ThrowIfInvalid("Sea Dragon Transpiler Invalid")
                .ThrowIfNotMatch("Sea Dragon Transpiler Not A Match")
                .Advance(1) //Move forward 1 index, so that we replace ldnull, and not the callvirt before it
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Set 'this' to top of stack, so next function takes that as input
                .Insert(Transpilers.EmitDelegate(GetSeaDragonReference)); //Take 'this' as input and return reference to the sea dragon this attack belongs to

            return matcher.InstructionEnumeration();
        }

        //Return a reference to the Sea Dragon the SeaDragonMeleeAttack component belongs to
        public static GameObject GetSeaDragonReference(SeaDragonMeleeAttack attack)
        {
            GameObject seaDragon = attack.seaDragon.gameObject;

            return seaDragon;
        }
    }
}