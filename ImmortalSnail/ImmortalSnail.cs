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
            CoroutineHost.StartCoroutine(GetBombPrefab());
        }

        private static IEnumerator GetBombPrefab()
        {
            ErrorMessage.AddMessage("Registering Bomb");

            //WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_06.prefab
            //IPrefabRequest request = PrefabDatabase.GetPrefabAsync("3c5abaf7-b18e-4835-8282-874763343d57");
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_06.prefab");
            yield return request;
            request.TryGetPrefab(out GameObject originalPrefab);

            GameObject resultPrefab = Object.Instantiate(originalPrefab);

            //Apply all changes to the resulting prefab, preparing it for our purposes
            EditBombPrefab(resultPrefab);

            var immortalSnail = new CustomPrefab("ImmortalSnail", "Immortal Snail", null);
            /*var bombPrefab = new CloneTemplate(immortalSnail.Info, TechType.PrecursorPrisonArtifact6)
            {
                //ModifyPrefab = prefab => prefab.GetComponentsInChildren<Mesh>();
            };*/

            immortalSnail.SetGameObject(resultPrefab);

            //Register the Immortal Snail to the game
            immortalSnail.Register();
        }

        [HarmonyPatch(typeof(PlayerTriggerAnimation), nameof(PlayerTriggerAnimation.OnTriggerEnter))]
        [HarmonyPostfix]
        private static void PostfixPlayerAnimationTrigger(Collider instance)
        {
            ErrorMessage.AddMessage($"{instance} collided with bomb.");
        }

        private static void EditBombPrefab(GameObject bombPrefab)
        {
            var bombColliders = bombPrefab.FindChild("colliders");
            var bombCoreController = bombPrefab.FindChild("alien_relic_02_core_ctrl");

            //Edit originalPrefab and then instantiate it?
            //CapsuleCollider seems to specify the range at which the animation is triggered
            //Thus, this should be the 'warning' radius, where the bomb starts animating and, if it gets much closer, explodes
            bombPrefab.GetComponent<CapsuleCollider>().radius = 3F;
            bombPrefab.GetComponent<CapsuleCollider>().height = 6F;

            //Enable CapsuleCollider's trigger on collision with player, so it can open up when close enough
            bombPrefab.GetComponent<CapsuleCollider>().isTrigger = true;

            //Make the actual physical collision sphere to the size of the model (when expanded), roughly
            bombColliders.GetComponent<SphereCollider>().radius = 0.2F;

            //Make the bomb itself overall slightly bigger, to make it easier to spot approaching
            //NOTE!! Specifically the core_02 is the object housing the model, so better I scale that up than the whole thing
            bombCoreController.GetComponent<Transform>().localScale *= 1.5F;

            //Access how to make the bomb glow (planning on having it glow more the closer it is to the player
            //Blinding 100% glow for when it goes critical and explodes
            Material coreMaterial = bombCoreController.FindChild("alien_relic_02_core").GetComponent<MeshRenderer>().material;

            //Set core to be green at 20% glow
            coreMaterial.SetColor(ShaderPropertyID._GlowColor, Color.green);
            coreMaterial.SetFloat(ShaderPropertyID._GlowStrength, 20F);
        }
    }
}
