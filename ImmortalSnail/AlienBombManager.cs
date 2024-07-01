using System;
using UnityEngine;

namespace ImmortalSnail
{
    public static class AlienBombManager
    {
        public static GameObject bombPrefab {  get; set; }
        public Vector3 bombPosition { get; set; }


        public static void SpawnBomb()
        {
            //Probably need to save a reference to this game object, to despawn it when it becomes unrendered
            GameObject alienBomb = UnityEngine.Object.Instantiate(bombprefab, bombPosition, Quaternion.identity);
        }

        public static void DespawnBomb(GameObject alienBomb)
        {
            //Probbaly need a reference to the bomb somewhere; perhaps in this mod class? Or the savedata?
            UnityEngine.Object.Destroy(alienBomb);
        }
    }
}
