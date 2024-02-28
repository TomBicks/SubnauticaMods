using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace CreatureConfigHealth
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class CreatureConfigHealthPlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfighealth";
        private const string pluginName = "Creature Config - Health";
        private const string versionString = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static ManualLogSource logger { get; private set; }

        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        private void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;
        }
    }

    [Menu("Creature Config - Health")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Toggle("Apply presets to aggressive fauna only?", Tooltip = "Apply presets to aggressive fauna only, so you can quickly apply sweeping changes without your thermoblade unable to kill a Peeper.")]
        public bool ApplyPresetsToAggressiveOnly = false;

        [Toggle("Make Cuddlefish invulnerable?", Tooltip = "Makes Cuddlefish invulnerable, so you don't have to worry about any danger to them.")]
        public bool CuddlefishInvunerable = false;

        [Toggle("Exclude changes to Bleeders?", Tooltip = "Excludes Bleeders' from health changes, as Bleeder's don't let go without being killed first or using the Propulsion or Repulsion Cannon.")]
        public bool ExcludeBleeder = false;

        [Slider("Health Presets", Min = 1F, Max = 8F, DefaultValue = 1F, Step = 1F, Id = "HealthPreset",
            Tooltip = "The health multiplier preset you wish to use if you want to quickly change all damage values. \n" +
            "Keep in mind that changes made to individual creatures below will not take effect unless you select preset 1, Custom. \n" +
            "1 = Custom, any individual changes made below will take effect \n" +
            "2 = One-Hit, Set creatures have 1 health \n" +
            "3 = Very Easy, Set creatures have 50% less health \n" +
            "4 = Easy, Set creatures have 25% less health \n" +
            "5 = Default, Set creatures have default health \n" +
            "6 = Hard, Set creatures have 25% more health \n" +
            "7 = Very Hard, Set creatures heave 50% more health \n" +
            "8 = Invulnerable, Set creatures are invulnerable and cannot be damaged")]
        public float HealthPreset = 1.0F;

        /*Health Ranges & Config Metrics
        HP = 1-80, Min-Max = 1-100, Step Value = 1
        HP = 81-400, Min-Max = 1-500, Step Value = 5
        HP = 401-2000, Min-Max = 1-2500, Step Value = 25
        HP = 2001-10000, Min-Max = 1-12500, Step Value = 125*/

        #region Ampeel
        [Slider("Ampeel Health", Min = 1F, Max = 12500F, DefaultValue = 3000F, Step = 125F, Id = "AmpeelHP")]
        public float AmpeelHP = 3000F;
        #endregion

        #region Biter
        [Slider("Biter Health", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "BiterHP")]
        public float BiterHP = 10F;
        #endregion

        #region Bladderfish
        [Slider("Bladderfish Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "BladderfishHP")]
        public float BladderfishHP = 30F;
        #endregion

        #region Bleeder
        [Slider("Bleeder Health", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "BleederHP", Tooltip = "Bleeder's don't let go without being killed first, or using the Propulsion or Repulsion Cannon, so keep this in mind if setting their health higher.")]
        public float BleederHP = 10F;
        #endregion

        #region Blighter
        [Slider("Blighter Health", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "BlighterHP")]
        public float BlighterHP = 10F;
        #endregion

        #region Blood Crawler
        [Slider("Blood Crawler Health", Min = 1F, Max = 100F, DefaultValue = 50F, Step = 1F, Id = "BloodCrawlerHP")]
        public float BloodCrawlerHP = 50F;
        #endregion

        #region Boneshark
        [Slider("Boneshark Health", Min = 1F, Max = 500F, DefaultValue = 200F, Step = 5F, Id = "BonesharkHP")]
        public float BonesharkHP = 200F;
        #endregion

        #region Boomerang
        [Slider("Boomerang Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "BoomerangHP")]
        public float BoomerangHP = 30F;
        #endregion

        #region Cave Crawler
        [Slider("Cave Crawler Health", Min = 1F, Max = 100F, DefaultValue = 50F, Step = 1F, Id = "CaveCrawlerHP")]
        public float CaveCrawlerHP = 50F;
        #endregion

        #region Crabsnake
        [Slider("Crabsnake Health", Min = 1F, Max = 500F, DefaultValue = 300F, Step = 5F, Id = "CrabsnakeHP")]
        public float CrabsnakeHP = 300F;
        #endregion

        #region Crabsquid
        [Slider("Crabsquid Health", Min = 1F, Max = 2500F, DefaultValue = 500F, Step = 25F, Id = "CrabsquidHP")]
        public float CrabsquidHP = 500F;
        #endregion

        #region Crashfish
        [Slider("Crashfish Health", Min = 1F, Max = 100F, DefaultValue = 25F, Step = 1F, Id = "CrashfishHP")]
        public float CrashfishHP = 25F;
        #endregion

        #region Crimson Ray
        [Slider("Crimson Ray Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "CrimsonRayHP")]
        public float CrimsonRayHP = 100F;
        #endregion

        #region Cuddlefish
        [Slider("Cuddlefish Health", Min = 1F, Max = 12500F, DefaultValue = 10000F, Step = 125F, Id = "CuddlefishHP")]
        public float CuddlefishHP = 10000F;
        #endregion

        #region Eyeye
        [Slider("Eyeye Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "EyeyeHP")]
        public float EyeyeHP = 30F;
        #endregion

        #region Floater
        [Slider("Floater Health", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "FloaterHP")]
        public float FloaterHP = 40F;
        #endregion

        #region Garryfish
        [Slider("Garryfish Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "GarryfishHP")]
        public float GarryfishHP = 30F;
        #endregion

        #region Gasopod
        [Slider("Gasopod Health", Min = 1F, Max = 500F, DefaultValue = 300F, Step = 5F, Id = "GasopodHP")]
        public float GasopodHP = 300F;
        #endregion

        #region Ghostray
        [Slider("Ghostray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "GhostrayHP")]
        public float GhostrayHP = 100F;
        #endregion

        #region Ghost Leviathan
        [Slider("Ghost Leviathan Health", Min = 1F, Max = 12500F, DefaultValue = 8000F, Step = 125F, Id = "GhostLeviathanHP")]
        public float GhostLeviathanHP = 8000F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Ghost Leviathan Juv. Health", Min = 1F, Max = 12500F, DefaultValue = 8000F, Step = 125F, Id = "GhostLeviathanJuvenileHP")]
        public float GhostLeviathanJuvenileHP = 8000F;
        #endregion

        #region Holefish
        [Slider("Holefish Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "HolefishHP")]
        public float HolefishHP = 30F;
        #endregion

        #region Hoopfish
        [Slider("Hoopfish Health", Min = 1F, Max = 100F, DefaultValue = 20F, Step = 1F, Id = "HoopfishHP")]
        public float HoopfishHP = 20F;
        #endregion

        #region Hoverfish
        [Slider("Hoverfish Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "HoverfishHP")]
        public float HoverfishHP = 30F;
        #endregion

        #region Jellyray
        [Slider("Jellyray Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "JellyrayHP")]
        public float JellyrayHP = 100F;
        #endregion

        #region Lava Larva
        [Slider("Lava Larva Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "LavaLarvaHP")]
        public float LavaLarvaHP = 100F;
        #endregion

        #region Lava Lizard
        [Slider("Lava Lizard Health", Min = 1F, Max = 500F, DefaultValue = 200F, Step = 5F, Id = "LavaLizardHP")]
        public float LavaLizardHP = 200F;
        #endregion

        #region Magmarang
        [Slider("Magmarang Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "MagmarangHP")]
        public float MagmarangHP = 30F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "MesmerHP")]
        public float MesmerHP = 100F;
        #endregion

        #region Oculus
        [Slider("Oculus Health", Min = 1F, Max = 100F, DefaultValue = 20F, Step = 1F, Id = "OculusHP")]
        public float OculusHP = 20F;
        #endregion

        #region Peeper
        [Slider("Peeper Health", Min = 1F, Max = 100F, DefaultValue = 20F, Step = 1F, Id = "PeeperHP")]
        public float PeeperHP = 20F;
        #endregion

        #region Rabbit Ray
        [Slider("Rabbit Ray Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "RabbitRayHP")]
        public float RabbitRayHP = 100F;
        #endregion

        #region Reaper Leviathan
        [Slider("Reaper Leviathan Health", Min = 1F, Max = 12500F, DefaultValue = 5000F, Step = 125F, Id = "ReaperLeviathanHP")]
        public float ReaperLeviathanHP = 5000F;
        #endregion

        #region Red Eyeye
        [Slider("Red Eyeye Health", Min = 1F, Max = 100F, DefaultValue = 25F, Step = 1F, Id = "RedEyeyeHP")]
        public float RedEyeyeHP = 30F;
        #endregion

        #region Reginald
        [Slider("Reginald Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "ReginaldHP")]
        public float ReginaldHP = 30F;
        #endregion

        #region River Prowler
        [Slider("River Prowler Health", Min = 1F, Max = 500F, DefaultValue = 200F, Step = 5F, Id = "RiverProwlerHP")]
        public float RiverProwlerHP = 200F;
        #endregion

        #region Sand Shark
        [Slider("Sand Shark Health", Min = 1F, Max = 500F, DefaultValue = 250F, Step = 5F, Id = "SandSharkHP")]
        public float SandSharkHP = 250F;
        #endregion

        #region Sea Dragon Leviathan
        [Slider("Sea Dragon Leviathan Health", Min = 1F, Max = 125F, DefaultValue = 5000F, Step = 125F, Id = "SeaDragonLeviathanHP")]
        public float SeaDragonLeviathanHP = 5000F;
        #endregion

        #region Sea Treader Leviathan
        [Slider("Sea Treader Leviathan Health", Min = 1F, Max = 12500F, DefaultValue = 3000F, Step = 125F, Id = "SeaTreaderLeviathanHP")]
        public float SeaTreaderLeviathanHP = 3000F;
        #endregion

        #region Shuttlebug
        [Slider("Shuttlebug Health", Min = 1F, Max = 100F, DefaultValue = 50F, Step = 1F, Id = "ShuttlebugHP")]
        public float ShuttlebugHP = 50F;
        #endregion

        #region Spadefish
        [Slider("Spadefish Health", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "SpadefishHP")]
        public float SpadefishHP = 30F;
        #endregion

        #region Spinefish
        [Slider("Spinefish Health", Min = 1F, Max = 100F, DefaultValue = 20F, Step = 1F, Id = "SpinefishHP")]
        public float SpinefishHP = 20F;
        #endregion

        #region Skyray
        [Slider("Skyray Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "SkyrayHP")]
        public float SkyrayHP = 100F;
        #endregion

        #region Stalker
        [Slider("Stalker Health", Min = 1F, Max = 500F, DefaultValue = 300F, Step = 5F, Id = "StalkerHP")]
        public float StalkerHP = 300F;
        #endregion

        #region Warper
        [Slider("Warper Health", Min = 1F, Max = 500F, DefaultValue = 100F, Step = 5F, Id = "WarperHP")]
        public float WarperHP = 100F;
        #endregion
    }
}
