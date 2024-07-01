using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using static ImmortalSnail.ImmortalSnailPlugin;
using static ImmortalSnail.AlienBombManager;
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

            //Register Story Goals for the mod
            RegisterStoryGoals();

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
            //Can probably disable the glass of the lab container and rotate the top half to be on the ground nearby, as if the glass broke and the top 'fell off'?
        }

        private static void RegisterStoryGoals()
        {
            /*Story progression will be
             - Aurora explodes; thus condition is met to release and spawn the bomb
             - PDA warning about the bomb (though with no idea what it is) will also be complete, with a delay
             - After the warning, once the bomb has gotten close enough to be rendered as an object, then a second PDA warning will be triggered,
               telling the player that the PDA recognises it's a bomb and to avoid it at all costs
             */

            #region Alien Bomb Released
            //Register story goal for when the bomb is released as a result of the Aurora exploding ('Story_AuroraWarning4' is the story goal for that)
            StoryGoalHandler.RegisterCompoundGoal("AlienBombReleased", Story.GoalType.PDA, delay: 30f, requiredGoals: "Story_AuroraWarning4");
            
            //Set PDA message and voiceline to play when the bomb is released, warning *something* is coming
            //PDAHandler.AddLogEntry("AlienBombReleased", "AlienBombReleased", sound);
            LanguageHandler.SetLanguageLine("AlienBombReleased", "Warning. Detecting massive energy signature steadily approaching. Continuing to monitor.", "English");
            
            //Activate the bomb tracking the player once it's released
            StoryGoalHandler.RegisterCustomEvent("AlienBombReleased", () =>
            {
                //Code to activate/spawn in the bomb at the mountains; will need to simulate it when not nearby the player, not unlike Persistent Reapers
                //This effectively activates the mod altogether

            });
            #endregion

            #region Alien Bomb Near
            //Register story goal for when the bomb is near enough as to be rendered in for the first time (goal will be manually triggered by the bomb's tracking code)
            //StoryGoalHandler.RegisterCompoundGoal("AlienBombNearby", Story.GoalType.PDA, delay: 5f, requiredGoals: "AlienBombReleased");
            new StoryGoal("AlienBombNearby", Story.GoalType.PDA, delay: 5f);

            //Set PDA message and voiceline to play when the bomb is nearby for the first time, warning the play to stay away from *whatever it is*
            //PDAHandler.AddLogEntry("AlienBombNearby", "AlienBombNearby", sound);
            LanguageHandler.SetLanguageLine("AlienBombNearby", "Emergency. Scans indicate approaching energy signature is locked onto your position and carries enough potential energy to destroy the entire planet. Avoid at all costs.", "English");
            #endregion

            #region Alien Bomb HUD Chip Ready
            //Register story goal for when the bomb is first nearby, after a large delay
            //NOTE!! delay will be larger than 120; just for testing purposes right now
            StoryGoalHandler.RegisterCompoundGoal("AlienBombHUDChipReady", Story.GoalType.PDA, delay: 120f, requiredGoals: "AlienBombNearby");

            //Unlock the bomb detector HUD chip, after a large delay of the player having to first deal with them being nearby
            StoryGoalHandler.RegisterOnGoalUnlockData("AlienBombHUDChipReady", blueprints: new UnlockBlueprintData[]
            {
                //new UnlockBlueprintData() {techType = TechType.AlienBombHUDChip, unlockType = UnlockBlueprintData.UnlockType.Available}
                new UnlockBlueprintData() {techType = TechType.MapRoomHUDChip, unlockType = UnlockBlueprintData.UnlockType.Available}
            });
            #endregion

            #region Alien Bomb HUD Chip Built
            //Register story goal for when the HUD chip to track the bomb has been created, warning it can't accurately track its location, but can provide a roguh estimate every 30 minutes.
            //StoryGoalHandler.RegisterItemGoal("AlienBombHUDChipBuilt", Story.GoalType.PDA, TechType.AlienBombHUDChip);
            StoryGoalHandler.RegisterItemGoal("AlienBombHUDChipBuilt", Story.GoalType.PDA, TechType.MapRoomHUDChip);
            #endregion
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

            //Set the prefab for the bomb
            bombPrefab = resultPrefab;
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
