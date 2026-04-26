using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;

namespace CreatureConfigDamage;

internal class DamageHandler
{
    //Modies the damage value of a creature prefab's MeleeAttack component (several creatures use the same implementation)
    private static void ModifyGenericMeleeAttack(ref GameObject creaturePrefab, float damage)
    {
        creaturePrefab.GetComponent<MeleeAttack>().biteDamage = damage;
        ErrorMessage.AddError($"Damage of {damage} assigned for {creaturePrefab} MeleeAttack component");
        Plugin.Logger.LogError($"Damage of {damage} assigned for {creaturePrefab} MeleeAttack component");
    }

    //Modifies the damage value of any component of a creature that *isn't* a MeleeAttack component, hence why the field of component itself has to be passed in
    private static void ModifyUniqueAttack(ref float componentDamageField, float damage)
    {
        componentDamageField = damage;
        ErrorMessage.AddError($"Damage of {damage} assigned for custom component");
        Plugin.Logger.LogError($"Damage of {damage} assigned for custom component");
    }

    //Function to iterate over the list of creature damage defaults to assign values
    public static IEnumerator Iterate()
    {
        foreach ((TechType techType, AttackInfo[] attacks) in AttackData.creatureAttacks)
        {
            Plugin.Logger.LogWarning($"{techType} has {attacks.Count()} attack(s)");
            ErrorMessage.AddMessage($"{techType} has {attacks.Count()} attack(s)");

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject prefab = task.GetResult();
            ErrorMessage.AddError($"{prefab}");
            Plugin.Logger.LogWarning(prefab);

            //By now, we've grabbed our TechType's prefab gameobject; now it's time to iterate over the creature's attacks to apply their changes to said gameobject
            foreach (AttackInfo attack in attacks)
            {
                Plugin.Logger.LogWarning($"Attacks values are {attack.attackKey}, {attack.defaultDamage}, and {attack.isGenericAttack}");
                ErrorMessage.AddMessage($"Attacks values are {attack.attackKey}, {attack.defaultDamage}, and {attack.isGenericAttack}");
                if (attack.isGenericAttack)
                {
                    //ModifyGenericMeleeAttack(ref prefab, attack.defaultDamage);
                    ModifyGenericMeleeAttack(ref prefab, attack.defaultDamage);
                }
                else
                {
                    //If the attack isn't generic, *then* we need to use the attackKeys to figure out the custom code to implement the damage changes
                    switch (attack.attackKey)
                    {
                        case "AmpeelBite":
                            ErrorMessage.AddError($"Attack {attack.attackKey} has damage of {attack.defaultDamage}");
                            Plugin.Logger.LogWarning($"Attack {attack.attackKey} has damage of {attack.defaultDamage}");
                            break;
                        case "GasPodPoison":
                            ModifyUniqueAttack(ref prefab.GetComponent<GasPod>().damagePerSecond, attack.defaultDamage);
                            break;
                        default:
                            Plugin.Logger.LogError($"Error! Attack {attack.attackKey} for {techType} has no implementation!");
                            break;
                    }
                }
            }
        }
    }
}
