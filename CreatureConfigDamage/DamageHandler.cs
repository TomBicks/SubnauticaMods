using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Nautilus.Handlers;
using UnityEngine;

namespace CreatureConfigDamage;

internal class DamageHandler
{
    private static float CalculateDamage(AttackInfo attack)
    {
        //Set the attack default
        float damage = attack.defaultDamage;

        //Obtain preset and determine which damage value to assign according to the preset
        float preset = Plugin.config.DamagePreset;

        switch (preset)
        {
            //Custom, apply individual custom changes
            case 1:
                damage = attack.configValue();
                break;
            //Sandbox, make all damage values 1
            case 2:
                damage = 1;
                break;
            //Damage Presets 3,4,5,6,7, multiply default damage values by a percentage, based on the preset selected
            //5 is Default, damage value is reset to default
            case float n when n >= 3 && n <= 7:
                damage = (preset - 1) / 4 * attack.defaultDamage;
                break;
            //Sudden Death, make all damage values 1000
            case 8:
                damage = 1000;
                break;
            default:
                Plugin.Logger.LogError($"Preset {preset} not recognised!");
                break;
        }

        //Return attack damage value to assign
        return damage;
    }

    //Modies the damage value of a creature prefab's MeleeAttack component (several creatures use the same implementation)
    private static void ModifyGenericMeleeAttack(ref GameObject creaturePrefab, AttackInfo attack)
    {
        float damage = CalculateDamage(attack);
        creaturePrefab.GetComponent<MeleeAttack>().biteDamage = damage;

        ErrorMessage.AddError($"Damage of {damage} assigned for {creaturePrefab} MeleeAttack component");
        Plugin.Logger.LogWarning($"Damage of {damage} assigned for {creaturePrefab} MeleeAttack component");
    }

    //Modifies the damage value of any component of a creature that *isn't* a MeleeAttack component, hence why the field of component itself has to be passed in
    private static void ModifyUniqueAttack(ref float componentDamageField, AttackInfo attack)
    {
        float damage = CalculateDamage(attack);
        componentDamageField = damage;

        ErrorMessage.AddError($"Damage of {damage} assigned for component for {attack.attackKey}");
        Plugin.Logger.LogWarning($"Damage of {damage} assigned for component for {attack.attackKey}");
    }

    //Function to iterate over the list of creature damage defaults to assign values
    public static IEnumerator ApplyDamageChanges(WaitScreenHandler.WaitScreenTask task)
    {
        task.Status = "Applying creature damage changes";
        foreach ((TechType techType, AttackInfo[] attacks) in AttackData.creatureAttacks)
        {
            Plugin.Logger.LogError($"{techType} has {attacks.Count()} attack(s)");
            ErrorMessage.AddMessage($"{techType} has {attacks.Count()} attack(s)");

            CoroutineTask<GameObject> prefabTask = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return prefabTask;

            GameObject prefab = prefabTask.GetResult();
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
                    ModifyGenericMeleeAttack(ref prefab, attack);
                }
                else
                {
                    //If the attack isn't generic, *then* we need to use the attackKeys to figure out the custom code to implement the damage changes
                    switch (attack.attackKey)
                    {
                        case "AmpeelShock":
                            ModifyUniqueAttack(ref prefab.GetComponent<ShockerMeleeAttack>().electricalDamage, attack);
                            break;
                        case "AmpeelCyclopsShock":
                            ModifyUniqueAttack(ref prefab.GetComponent<ShockerMeleeAttack>().cyclopsDamage, attack);
                            break;
                        case "BleederSuck":
                            ModifyUniqueAttack(ref prefab.GetComponent<AttachAndSuck>().leechDamage, attack);
                            break;
                        case "CrabsnakeBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<CrabsnakeMeleeAttack>().biteDamage, attack);
                            break;
                        case "CrabsnakeGrabBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<CrabsnakeMeleeAttack>().cinematicAttackAdditionalDamage, attack);
                            break;
                        case "CrabsnakeSeamothBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<CrabsnakeMeleeAttack>().seamothDamage, attack);
                            break;
                        case "CrashfishExplosion":
                            ModifyUniqueAttack(ref prefab.GetComponent<Crash>().maxDamage, attack);
                            break;
                        case "GasPodPoison":
                            ModifyUniqueAttack(ref prefab.GetComponent<GasPod>().damagePerSecond, attack);
                            break;
                        case "GhostBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, attack);
                            break;
                        case "GhostCyclopsBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, attack);
                            break;
                        case "GhostJuvBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<GhostLeviathanMeleeAttack>().biteDamage, attack);
                            break;
                        case "GhostJuvCyclopsBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<GhostLeviathanMeleeAttack>().cyclopsDamage, attack);
                            break;
                        case "LavaLizardLavaRock":
                            ModifyUniqueAttack(ref prefab.GetComponent<LavaLiazardRangedAttack>().attackTypes[0].ammoPrefab.GetComponent<LavaMeteor>().damage, attack);
                            break;
                        case "ReaperBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<ReaperMeleeAttack>().biteDamage, attack);
                            break;
                        case "ReaperCyclopsBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<ReaperMeleeAttack>().cyclopsDamage, attack);
                            break;
                        case "SeaDragonBite":
                            ModifyUniqueAttack(ref prefab.GetComponent<SeaDragonMeleeAttack>().biteDamage, attack);
                            break;
                        case "SeaDragonSwat":
                            ModifyUniqueAttack(ref prefab.GetComponent<SeaDragonMeleeAttack>().swatAttackDamage, attack);
                            break;
                        case "SeaDragonShove":
                            ModifyUniqueAttack(ref prefab.GetComponent<SeaDragonMeleeAttack>().shoveAttackDamage, attack);
                            break;
                        case "SeaDragonBurningChunk":
                            ModifyUniqueAttack(ref prefab.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab.GetComponent<BurningChunk>().fireDamage, attack);
                            break;
                        case "SeaDragonLavaMeteor":
                            ModifyUniqueAttack(ref prefab.GetComponent<RangedAttackLastTarget>().attackTypes[1].ammoPrefab.GetComponent<LavaMeteor>().damage, attack);
                            break;
                        case "SeaTreaderPeck":
                            ModifyUniqueAttack(ref prefab.GetComponent<SeaTreaderMeleeAttack>().damage, attack);
                            break;
                        case "TigerPlantThorn":
                            ModifyUniqueAttack(ref prefab.GetComponent<RangeAttacker>().damage, attack);
                            break;
                        case "WarperClaw":
                            ModifyUniqueAttack(ref prefab.GetComponent<WarperMeleeAttack>().biteDamage, attack);
                            break;
                        case "WarperWarpBall":
                            ModifyUniqueAttack(ref prefab.GetComponent<RangedAttackLastTarget>().attackTypes[0].ammoPrefab.GetComponent<WarpBall>().damage, attack);
                            break;
                        default:
                            Plugin.Logger.LogError($"Error! Attack {attack.attackKey} for {techType} has no implementation!");
                            break;
                    }
                }
            }
        }
        yield return null;
    }
}
