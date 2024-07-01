using System;
using UnityEngine;

namespace ImmortalSnail
{
    public static class AlienBombManager
    {
        //Prefab refers to the prefab we spawn; Object is a direct reference to the instance of the prefab spawned
        public static GameObject bombPrefab {  get; set; }
        public static GameObject bombObject { get; set; }
        public static Vector3 bombPosition { get; private set; }
        //This will be enabled, and remain enabled, only once; when the bomb is first released and tracking the player
        public static bool bombHunting { get; private set; }

        public static void StartBombHunting()
        {
            bombHunting = true;
        }

        public static void SpawnBomb()
        {
            //Probably need to save a reference to this game object, to despawn it when it becomes unrendered
            bombObject = UnityEngine.Object.Instantiate(bombPrefab, bombPosition, Quaternion.identity);
        }

        public static void DespawnBomb(GameObject alienBomb)
        {
            //Probbaly need a reference to the bomb somewhere; perhaps in this mod class? Or the savedata?
            UnityEngine.Object.Destroy(alienBomb);
        }

        public static void UpdateBombPosition(GameObject alienBomb)
        {
            bombPosition = alienBomb.gameObject.transform.position;
        }
    }
}
