using System;
using HarmonyLib;
using UnityEngine;

namespace ImmortalSnail
{
    public static class AlienBombManager
    {
        //Prefab refers to the prefab we spawn; Object is a direct reference to the instance of the prefab spawned
        public static GameObject bombPrefab {  get; set; }
        public static GameObject bombObject { get; set; }
        public static Vector3 bombPosition { get; private set; }

        private static float moveSpeed = 1.0f;
        private static float turnSpeed = 1.0f;

        //This will be enabled, and remain enabled, only once; when the bomb is first released and tracking the player
        public static bool bombHunting { get; private set; }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void PostfixPlayerUpdate(Player __instance)
        {
            UpdateBombRotation();
        }

        public static void StartBombHunting()
        {
            bombHunting = true;
        }

        public static void SpawnBomb()
        {
            //Probably need to save a reference to this game object, to despawn it when it becomes unrendered
            bombObject = UnityEngine.Object.Instantiate(bombPrefab, bombPosition, Quaternion.identity);
        }

        public static void DespawnBomb()
        {
            //Probbaly need a reference to the bomb somewhere; perhaps in this mod class? Or the savedata?
            UnityEngine.Object.Destroy(bombObject);
        }

        public static void UpdateBombPosition()
        {
            bombPosition = bombObject.gameObject.transform.position;
        }

        public static void UpdateBombRotation()
        {
            //https://discussions.unity.com/t/how-can-i-make-a-game-object-look-at-another-object/98932
            //Determine the rotation required to face the player
            var lookRotation = Quaternion.LookRotation((Player.main.transform.position - bombPosition).normalized);
            //Slowly interpolate between where the bomb is currently facing and where it needs to face the player
            bombObject.transform.rotation = Quaternion.Slerp(bombObject.transform.rotation, lookRotation, turnSpeed);
        }
    }
}
