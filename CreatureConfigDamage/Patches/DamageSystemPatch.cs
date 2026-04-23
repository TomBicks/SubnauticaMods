using HarmonyLib;
using UnityEngine;

namespace CreatureConfigDamage.Patches;

[HarmonyPatch(typeof(DamageSystem))]
internal class DamageSystemPatch
{
    //Patch damage calculations, "ignoring" the reduction in damage from the reinforced diving suit, if a creature has been set by the user to ignore it.
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
    [HarmonyPostfix]
    public static float CalculateArmourDamage(float damage, DamageType type, GameObject target, GameObject dealer)
    {
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