using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace CreatureConfigDamage.Patches;

[HarmonyPatch(typeof(DamageSystem))]
internal class DamageSystemPatch
{
    //Patch damage calculations, "ignoring" the reduction in damage from the reinforced diving suit, if a creature has been set by the user to ignore it.
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
    [HarmonyPostfix]
    public static float PostfixCalculateDamage(float damage, DamageType type, GameObject target, GameObject dealer)
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
                if (armourValue > 0 && Plugin.config.IgnoreArmour.ContainsKey(dealerTechType))
                {
                    if (Plugin.config.IgnoreArmour[dealerTechType])
                    {
                        //Calculate what to multiply the reduced damage by to restore it back to full damage
                        float originalDamageMultiplier = 1 / (1 - armourValue);
                        //TODO!! "Harmony003" problem???
                        damage *= originalDamageMultiplier;

                        return damage;
                    }
                }
            }
        }

        return damage; //Returns the damage value calculated at the end of the patched function, not the damage given at the start
    }
}
