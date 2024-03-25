using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using static ImmortalSnail.ImmortalSnailPlugin;
using UnityEngine;
using System.Collections;
using UWE;

namespace ImmortalSnail
{
    [HarmonyPatch]
    internal class ImmortalSnail
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void PostfixPlayer(Player __instance)
        {
            /*var immortalSnail = new CustomPrefab("ImmortalSnail", "Immortal Snail", null);

            // Creates a clone of the Crashfish prefab and colors it green, then set the new prefab as our Immortal Snail's game object.          
            var immortalSnailPrefab = new CloneTemplate(immortalSnail.Info, TechType.Crash)
            {
                ModifyPrefab = prefab => prefab.GetComponentsInChildren<Renderer>().ForEach(r => r.materials.ForEach(m => m.color = Color.green))
            };
            immortalSnail.SetGameObject(immortalSnailPrefab);

            immortalSnail.SetSpawns(new SpawnLocation(new UnityEngine.Vector3(0, -30, 0)));

            //Register the Immortal Snail to the game
            immortalSnail.Register();*/

            CoroutineHost.StartCoroutine(GetBombPrefab());
            //GetBombPrefab();
        }

        private static IEnumerator GetBombPrefab()
        {
            ErrorMessage.AddMessage("Registering Bomb");

            //ERROR! Alien Bomb has no CraftData, so have to use PrefabDatabase instead of this
            /*CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.PrecursorPrisonArtifact6, true);
            yield return task;
            GameObject prefab = task.GetResult();*/

            //WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_06.prefab
            //IPrefabRequest request = PrefabDatabase.GetPrefabAsync("3c5abaf7-b18e-4835-8282-874763343d57");
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_06.prefab");
            yield return request;
            request.TryGetPrefab(out GameObject originalPrefab);
            GameObject resultPrefab = Object.Instantiate(originalPrefab);

            var immortalSnail = new CustomPrefab("ImmortalSnail", "Immortal Snail", null);
            var bombPrefab = new CloneTemplate(immortalSnail.Info, TechType.PrecursorPrisonArtifact6)
            {
                //ModifyPrefab = prefab => prefab.GetComponentsInChildren<Mesh>();
            };

            immortalSnail.SetGameObject(resultPrefab);

            immortalSnail.SetSpawns(new SpawnLocation(new UnityEngine.Vector3(0, -30, 0)));

            //Register the Immortal Snail to the game
            immortalSnail.Register();
        }
    }
}
