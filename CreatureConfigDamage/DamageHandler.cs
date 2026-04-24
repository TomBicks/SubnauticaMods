using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;

namespace CreatureConfigDamage;

internal class DamageHandler
{
    //TODO!! instead of a dictionary in DamageDefaults, just make a list of techtypes, with each techtype being tupled with an array/list of AttackInfo for the creature
    //Then, when we start the mod and register the changes, all it has to do is go down the list for each techtype to retrieve, before grabbing each attack it needs to change (and whether it's generic too!)
    //Create list of creatures and their info to add to the game
    private readonly List<TechType> targetCreatures = new List<TechType>()
    {
        TechType.Peeper,
        TechType.Biter
    };

    public void ModifyCreaturePrefabDamage(bool isGeneric)
    {
        foreach (TechType targetTechType in targetCreatures)
        {
            if (isGeneric)
            {
                //CoroutineHost.StartCoroutine(ModifyGenericMeleeAttack(targetTechType));
            }
        }
    }

    //Modies the damage value of a creature prefab's MeleeAttack component (several creatures use the same implementation)
    private static void ModifyGenericMeleeAttack(ref GameObject creaturePrefab, float damage)
    {
        creaturePrefab.GetComponent<MeleeAttack>().biteDamage = damage;
        ErrorMessage.AddError($"Damage of {damage} assigned for {creaturePrefab}");
        Plugin.Logger.LogError($"Damage of {damage} assigned for {creaturePrefab}");
    }

    //Passed custom damage value and default damage value from dictionary; returns either the damage value to assign, or -1 if there is no such key in the dictionary
    //Both ChangeAttack methods use this code; reduces redundant code
    public static float CalculateDmgToAssign(float __customDmgValue, string __defaultDmgValueKey)
    {
        //First, check if key for default damage value actually exists in the dictionary
        if (DamageDefaults.defaults.ContainsKey(__defaultDmgValueKey))
        {
            //If it exists, assign the default damage value to a variable
            float defaultDmgValue = DamageDefaults.defaults[__defaultDmgValueKey];

            //Store value to assign as new damage for the selected creature's unique attack
            //Default value is the default damage for the creature's unique attack
            float dmgValueToAssign = defaultDmgValue;

            //Obtain preset and determine which damage value to assign according to the preset
            float preset = Plugin.config.DamagePreset;

            switch (preset)
            {
                //Custom, apply individual custom changes
                case 1:
                    dmgValueToAssign = __customDmgValue;
                    break;

                //Sandbox, make all damage values 1
                case 2:
                    dmgValueToAssign = 1;
                    break;

                //Damage Presets 3,4,5,6,7, multiply default damage values by a percentage, based on the preset selected
                //5 is Default, damage value is reset to default
                case float n when n >= 3 && n <= 7:
                    dmgValueToAssign = (preset - 1) / 4 * defaultDmgValue;
                    break;

                //Sudden Death, make all damage values 1000
                case 8:
                    dmgValueToAssign = 1000;
                    break;

                default:
                    Plugin.Logger.Log(LogLevel.Error, $"Preset {preset} not recognised!");
                    break;
            }

            //Return attack damage value to assign
            return dmgValueToAssign;
        }
        else
        {
            //Return -1 if the calculation failed and the key did not exist in the default damage dictionary
            Plugin.Logger.Log(LogLevel.Error, $"Default Damage Value Key {__defaultDmgValueKey} does not exist in the dictionary!");
            return -1.0f;
        }
    }

    //TODO!! Function to iterate over the list of creature damage defaults to assign values
    public static IEnumerator Iterate()
    {
        foreach ((TechType techType, AttackInfo[] attacks) in DamageDefaults.creatureAttacks)
        {
            Plugin.Logger.LogError($"{techType} has {attacks.Count()} attack(s)");
            ErrorMessage.AddMessage($"{techType} has {attacks.Count()} attack(s)");

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject prefab = task.GetResult();
            ErrorMessage.AddError($"{prefab}");
            Plugin.Logger.LogError(prefab);

            //By now, we've grabbed our TechType's prefab gameobject; now it's time to iterate over the attacks to apply their changes to said gameobject
            foreach (AttackInfo attack in attacks)
            {
                Plugin.Logger.LogError($"Attacks values are {attack.attackKey}, {attack.defaultDamage}, and {attack.isGenericAttack}");
                ErrorMessage.AddMessage($"Attacks values are {attack.attackKey}, {attack.defaultDamage}, and {attack.isGenericAttack}");
                if (attack.isGenericAttack)
                {
                    ModifyGenericMeleeAttack(ref prefab, attack.defaultDamage);
                }
                else
                {
                    //If the attack isn't generic, *then* we need to use the attackKeys to figure out the custom changes we need to make
                    switch (attack.attackKey)
                    {
                        case "AmpeelBite":
                            ErrorMessage.AddError($"Attack {attack.attackKey} has damage of {attack.defaultDamage}");
                            Plugin.Logger.LogError($"Attack {attack.attackKey} has damage of {attack.defaultDamage}");
                            break;
                    }
                }
            }
        }
    }
}
