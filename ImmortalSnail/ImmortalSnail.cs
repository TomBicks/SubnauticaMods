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

            //Edit originalPrefab and then instantiate it?
            //CapsuleCollider seems to specify the range at which the animation is triggered
            //Thus, this should be the 'warning' radius, where the bomb starts animating and, if it gets much closer, explodes
            resultPrefab.GetComponent<CapsuleCollider>().radius = 3F;
            resultPrefab.GetComponent<CapsuleCollider>().height = 6F;

            //Enable CapsuleCollider's trigger on collision with player, so it can open up when close enough
            resultPrefab.GetComponent<CapsuleCollider>().isTrigger = true;

            //Make the actual physical collision sphere to the size of the model (when expanded), roughly
            resultPrefab.FindChild("colliders").GetComponent<SphereCollider>().radius = 0.2F;

            //Make the bomb itself overall slightly bigger, to make it easier to spot approaching
            //NOTE!! Specifically the core_02 is the object housing the model, so better I scale that up than the whole thing
            resultPrefab.FindChild("alien_relic_02_core_ctrl").GetComponent<Transform>().localScale *=1.5F;


            var immortalSnail = new CustomPrefab("ImmortalSnail", "Immortal Snail", null);
            /*var bombPrefab = new CloneTemplate(immortalSnail.Info, TechType.PrecursorPrisonArtifact6)
            {
                //ModifyPrefab = prefab => prefab.GetComponentsInChildren<Mesh>();
            };*/

            immortalSnail.SetGameObject(resultPrefab);

            immortalSnail.SetSpawns(new SpawnLocation(new UnityEngine.Vector3(0, -30, 0)));

            //Register the Immortal Snail to the game
            immortalSnail.Register();
        }

        [HarmonyPatch(typeof(PlayerTriggerAnimation), nameof(PlayerTriggerAnimation.OnTriggerEnter))]
        [HarmonyPostfix]
        public static void PostfixPlayerAnimationTrigger(Collider __instance)
        {
            ErrorMessage.AddMessage("Test");
            if (__instance.gameObject.TryGetComponent<Player>(out Player player))
            {
                ErrorMessage.AddMessage($"Player collided {player}");
            }
        }
    }
}
