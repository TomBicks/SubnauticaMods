using System.Collections;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace CreatureConfigDamage.Patches;

//DEBUG!! This will eventually be handled in the wait screen handler stuff
[HarmonyPatch(typeof(Player))]
internal class PlayerPatch : MonoBehaviour
{
    [HarmonyPatch(nameof(Player.Start))]
    [HarmonyPostfix]
    public static void PlayerStart()
    {
        CoroutineHost.StartCoroutine(DamageHandler.Iterate());
        CoroutineHost.StartCoroutine(TestPrefabStuff());
    }

    private static IEnumerator TestPrefabStuff()
    {
        ErrorMessage.AddError("TestPrefabStuff");
        Plugin.Logger.LogError("TestPrefabStuff");
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.GasPod);
        yield return task;

        GameObject prefab = task.GetResult();
        ErrorMessage.AddError($"{prefab}");
        Plugin.Logger.LogError(prefab);
        //NOTE!! Can confirm; this makes all gaspods from here on deal 1 damage per second, meaning it works, rather than patching into the gaspod itself
        prefab.GetComponent<GasPod>().damagePerSecond = 1;

        //Instantiate the prefab with a random rotation 2 meters in front of the player camera:
        Instantiate(prefab, MainCamera.camera.transform.position + (MainCamera.camera.transform.forward * 2), UnityEngine.Random.rotation);
        ErrorMessage.AddError("Spawning Test GasPod");
        Plugin.Logger.LogError("Spawning Test GasPod");
    }
}
