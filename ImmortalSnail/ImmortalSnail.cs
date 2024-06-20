using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using static ImmortalSnail.ImmortalSnailPlugin;
using UnityEngine;
using System.Collections;
using UWE;
using Nautilus.Handlers;
using Story;

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

            //If bomb has been released, make sure it's not in the glass case, and that the glass case is breached
            BreakContainer();
        }

        [HarmonyPatch(typeof(StoryGoal), nameof(StoryGoal.Trigger))]
        [HarmonyPostfix]
        public static void PostfixStoryGoal(StoryGoal __instance)
        {
            logger.LogWarning($"key = {__instance.key}, goal type = {__instance.goalType}, delay = {__instance.delay}");
            ErrorMessage.AddDebug($"key = {__instance.key}, goal type = {__instance.goalType}, delay = {__instance.delay}");
        }

        public static void BreakContainer()
        {
            //prefabKey for the basic alien container is 'WorldEntities/Doodads/Precursor/Precursor_lab_container_01.prefab'
            //Unsure if there's a broken version of it though'
        }

        //NOTE!! The 'RadioRadiationSuit' message is triggered as soon as the player leaves the lifepod, with a delay of '21600'.
        //This is likely the same amount of time it takes for the Aurora to explode.
        //Then, when it has exploded, a 'UnlockRadiationSuit' Story goal triggers with a delay of 40
        //a 'RadSuit' Encyclopedia goal triggers with a delay of 3
        //a 'Goal_UnlockRadSuit' PDA goal triggers with a delay of 0
        //So, what this means is; after 21600 ticks, the Aurora explodes, triggering whatever this radio message is???
        //Then, it plays the PDA message immediately, followed 3 seconds later by the recipe unlocking.

        //It's unclear where the 40 delay one triggers though, or what its purpose is?
        //Perhaps it is started as the explosion begins, so 40 seconds after the explosion, the other two goals are triggered?

        //So, if I want the message to be sent a bit after the explosion, even after the radiation suit unlock, I'd set it later than that???
        //I do want to release the bomb right after the explosion though; the PDA message just won't occur immediately as it happens.

        private void RegisterStoryGoals()
        {
            /*Story progression will be
             - Aurora explodes; thus condition is met to release and spawn the bomb
             - PDA warning about the bomb will also be complete, with a delay
             - After the warning, if the player has *also* scanned the bomb already, another PDA message will play,
               telling the player that it's the bomb approaching them. Also grants them the HUD chip to locate it.  
             */
            //BUT!! What if the player hasn't scanned the bomb before it's released? Should I give the research to them after a set amount of time?

            #region Alien Bomb Released
            //Register goal to display PDA warning message about the bomb a little bit after the Aurora explosion
            StoryGoalHandler.RegisterCompoundGoal("AlienBombReleased", GoalType.PDA, delay: 30f, requiredGoals: "auroraWarning4");
            //Register PDA warning voice line (if I ever make a voice line; unlikely)
            //PDAHandler.AddLogEntry("AlienBombReleased", "AlienBombReleased", sound);
            //Set the English translation for PDA bomb warning
            LanguageHandler.SetLanguageLine("AlienBombReleased", "Warning. Detecting massive energy signature steadily approaching. Exercise caution.", "English");
            //Spawn in the bomb at the mountains once the aurora has exploded
            StoryGoalHandler.RegisterCustomEvent("AlienBombReleased", () =>
            {
                //Code to spawn in the bomb at the mountains; will need to simulate it when not nearby the player, not unlike Persistent Reapers
            });
            #endregion

            #region Alien Bomb Scanned
            //ERROR!! There is no goal for scans; have to determine another way to check if the bomb has been scanned
            //This will likely require a story goal, that I trigger once a manual check if its been scanned or not completes.
            //Register goal to display PDA message about the fact the approaching signal is the doomsday device
            StoryGoalHandler.RegisterCompoundGoal("AlienBombScanned", GoalType.PDA, delay: 30f, requiredGoals: "AlienBombReleased");
            //Unlock the bomb detector HUD chip if the bomb has been released AND the player has scanned the bomb eariler, when it was in the case.
            StoryGoalHandler.RegisterOnGoalUnlockData("AlienBombScanned", blueprints: new Story.UnlockBlueprintData[]
            {
                new UnlockBlueprintData() {techType = TechType.Seamoth, unlockType = Story.UnlockBlueprintData.UnlockType.Available},
            });
        }

        /*[HarmonyPatch(typeof(CrashedShipExploder), nameof(CrashedShipExploder.Update))]
        [HarmonyPostfix]
        //NOTE!! DON'T USE THIS; use Story Goals https://subnauticamodding.github.io/Nautilus/tutorials/story-goals.html
        public static void PostfixCrashedShip(CrashedShipExploder __instance)
        {
            if (__instance.IsExploded())
            {
                ErrorMessage.AddMessage("Testing Explosion Hook");
                logger.LogDebug("Testing Explosion Hook");
            }
        }*/

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
