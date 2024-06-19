﻿using HarmonyLib;
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
            MainCameraControl.main.ShakeCamera(4f, 8f, MainCameraControl.ShakeMode.Quadratic, 1.2f); //This Works!
            WorldForces.AddExplosion(new Vector3(0f, 0f, 0f), (double)new Utils.ScalarMonitor(0f).Get(), 8f, 5000f);
            CoroutineHost.StartCoroutine(GetBombPrefab());
        }

        //If this doesn't work, and I can't hook into its BroadcastMessage of 'OnShipExplode', perhaps I could just postfix it? That feels iffy though
        public void OnShipExplode()
        {
            ErrorMessage.AddMessage("Testing Explosion Hook");
            logger.LogDebug("Testing Explosion Hook");
        }

        [HarmonyPatch(typeof(CrashedShipExploder), nameof(CrashedShipExploder.Update))]
        [HarmonyPostfix]
        //NOTE!! DON'T USE THIS; use Story Goals https://subnauticamodding.github.io/Nautilus/tutorials/story-goals.html
        public static void PostfixCrashedShip(CrashedShipExploder __instance)
        {
            if (__instance.IsExploded())
            {
                ErrorMessage.AddMessage("Testing Explosion Hook");
                logger.LogDebug("Testing Explosion Hook");
            }
        }

        private static IEnumerator GetBombPrefab()
        {
            ErrorMessage.AddMessage("Registering Bomb");
            logger.LogDebug("Registering Bomb");

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
        private static void PostfixPlayerAnimationTrigger(Collider enterCollider)
        {
            ErrorMessage.AddMessage($"{enterCollider} collided with bomb.");
            logger.LogDebug($"{enterCollider} collided with bomb.");
        }

        private static void EditBombPrefab(GameObject bombPrefab)
        {
            //NOTE!! logger.LogInfo() doesn't seem to work here? Unsure why

            var bombColliders = bombPrefab.FindChild("colliders");
            ErrorMessage.AddMessage("Registering bomb collider");
            logger.LogDebug("Registering bomb collider");
            var bombCoreController = bombPrefab.FindChild("alien_relic_02_core_ctrl");
            ErrorMessage.AddMessage("Registering bomb core controller");
            logger.LogDebug("Registering bomb core controller");
            var bombCore = bombCoreController.FindChild("alien_relic_02_core");
            ErrorMessage.AddMessage("Registering bomb core");
            logger.LogDebug("Registering bomb core");

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
            Material coreMaterial = bombCore.GetComponent<MeshRenderer>().material;

            //Set core to be green at 20% glow
            coreMaterial.SetColor(ShaderPropertyID._GlowColor, Color.green);
            coreMaterial.SetFloat(ShaderPropertyID._GlowStrength, 20F);

            ErrorMessage.AddMessage("Iterating through bomb sliders...");
            logger.LogDebug("Iterating through bomb sliders...");
            //Iterate through each of the 8 outer sections of the bomb; make them each glow slightly, for visibility
            for (int i = 1; i < 9; i++)
            {
                ErrorMessage.AddMessage($"Finding alien_relic_02_slider_ctrl({i}).alien_relic_02_slider_0{i}");
                logger.LogDebug($"Finding alien_relic_02_slider_ctrl({i}).alien_relic_02_slider_0{i}");
                GameObject sliderObject = bombCore.FindChild($"alien_relic_02_slider_ctrl({i})").FindChild($"alien_relic_02_slider_0{i}");
                Material sliderMaterial = sliderObject.GetComponent<MeshRenderer>().material;
                sliderMaterial.SetFloat(ShaderPropertyID._GlowStrength, 6F);
            }
        }
    }
}
