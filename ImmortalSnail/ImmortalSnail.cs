using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using static ImmortalSnail.ImmortalSnailPlugin;
using UnityEngine;

namespace ImmortalSnail
{
    [HarmonyPatch]
    internal class ImmortalSnail
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void PostfixPlayer(Player __instance)
        {
            var immortalSnail = new CustomPrefab("ImmortalSnail", "Immortal Snail", null);

            // Creates a clone of the Crashfish prefab and colors it green, then set the new prefab as our Immortal Snail's game object.          
            var immortalSnailPrefab = new CloneTemplate(immortalSnail.Info, TechType.Crash)
            {
                ModifyPrefab = prefab => prefab.GetComponentsInChildren<Renderer>().ForEach(r => r.materials.ForEach(m => m.color = Color.green))
            };
            immortalSnail.SetGameObject(immortalSnailPrefab);

            //
            immortalSnail.SetSpawns(new SpawnLocation(new UnityEngine.Vector3(0, -30, 0)));

            //Register the Immortal Snail to the game
            immortalSnail.Register();
        }
    }
}
