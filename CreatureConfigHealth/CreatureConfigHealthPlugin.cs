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

        public static ManualLogSource logger;

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
        //NOTE!! Maybe have a boolean toggle as to whether the health changes should affect all creatures or just aggressive ones!
        //Maybe have another that can make the cuddlefish immortal?

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

        [Toggle("Apply presets to aggressive fauna only?")] 
        public bool ApplyPresetsToAggressiveOnly = false;

        [Toggle("Make Cuddlefish invulnerable?")]
        public bool CuddlefishInvunerable = false;

        #region Ampeel
        [Slider("Ampeel Health", Min = 1F, Max = 1F, DefaultValue = 3000F, Step = 1F, Id = "AmpeelHP")]
        public float AmpeelHP = 1F;
        #endregion

        #region Biter
        [Slider("Biter Health", Min = 1F, Max = 1F, DefaultValue = 10F, Step = 1F, Id = "BiterHP")]
        public float BiterHP = 1F;
        #endregion

        #region Bladderfish
        [Slider("Bladderfish Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "BladderfishHP")]
        public float BladderfishHP = 1F;
        #endregion

        #region Bleeder
        [Slider("Bleeder Health", Min = 1F, Max = 1F, DefaultValue = 10F, Step = 1F, Id = "BleederHP")]
        public float BleederHP = 1F;
        #endregion

        #region Blighter
        [Slider("Blighter Health", Min = 1F, Max = 1F, DefaultValue = 10F, Step = 1F, Id = "BlighterHP")]
        public float BlighterHP = 1F;
        #endregion

        #region Blood Crawler
        [Slider("Blood Crawler Health", Min = 1F, Max = 1F, DefaultValue = 50F, Step = 1F, Id = "BloodCrawlerHP")]
        public float BloodCrawlerHP = 1F;
        #endregion

        #region Boneshark
        [Slider("Boneshark Health", Min = 1F, Max = 1F, DefaultValue = 200F, Step = 1F, Id = "BonesharkHP")]
        public float BonesharkHP = 1F;
        #endregion

        #region Boomerang
        [Slider("Boomerang Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "BoomerangHP")]
        public float BoomerangHP = 1F;
        #endregion

        #region Cave Crawler
        [Slider("Cave Crawler Health", Min = 1F, Max = 1F, DefaultValue = 50F, Step = 1F, Id = "CaveCrawlerHP")]
        public float CaveCrawlerHP = 1F;
        #endregion

        #region Crabsnake
        [Slider("Crabsnake Health", Min = 1F, Max = 1F, DefaultValue = 300F, Step = 1F, Id = "CrabsnakeHP")]
        public float CrabsnakeHP = 1F;
        #endregion

        #region Crabsquid
        [Slider("Crabsquid Health", Min = 1F, Max = 1F, DefaultValue = 500F, Step = 1F, Id = "CrabsquidHP")]
        public float CrabsquidHP = 1F;
        #endregion

        #region Crashfish
        [Slider("Crashfish Health", Min = 1F, Max = 1F, DefaultValue = 25F, Step = 1F, Id = "CrashfishHP")]
        public float CrashfishHP = 1F;
        #endregion

        #region Crimson Ray
        [Slider("Crimson Ray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "CrimsonRayHP")]
        public float CrimsonRayHP = 1F;
        #endregion

        #region Cuddlefish
        [Slider("Cuddlefish Health", Min = 1F, Max = 1F, DefaultValue = 10000F, Step = 1F, Id = "CuddlefishHP")]
        public float CuddlefishHP = 1F;
        #endregion

        #region Eyeye
        [Slider("Eyeye Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "EyeyeHP")]
        public float EyeyeHP = 1F;
        #endregion

        #region Floater
        [Slider("Floater Health", Min = 1F, Max = 1F, DefaultValue = 40F, Step = 1F, Id = "FloaterHP")]
        public float FloaterHP = 1F;
        #endregion

        #region Garryfish
        [Slider("Garryfish Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "GarryfishHP")]
        public float GarryfishHP = 1F;
        #endregion

        #region Gasopod
        [Slider("Gasopod Health", Min = 1F, Max = 1F, DefaultValue = 300F, Step = 1F, Id = "GasopodHP")]
        public float GasopodHP = 1F;
        #endregion

        #region Ghostray
        [Slider("Ghostray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "GhostrayHP")]
        public float GhostrayHP = 1F;
        #endregion

        #region Ghost Leviathan
        [Slider("Ghost Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 8000F, Step = 1F, Id = "GhostLeviathanHP")]
        public float GhostLeviathanHP = 1F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Ghost Leviathan Juv. Health", Min = 1F, Max = 1F, DefaultValue = 8000F, Step = 1F, Id = "GhostLeviathanJuvenileHP")]
        public float GhostLeviathanJuvenileHP = 1F;
        #endregion

        #region Holefish
        [Slider("Holefish Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "HolefishHP")]
        public float HolefishHP = 1F;
        #endregion

        #region Hoopfish
        [Slider("Hoopfish Health", Min = 1F, Max = 1F, DefaultValue = 20F, Step = 1F, Id = "HoopfishHP")]
        public float HoopfishHP = 1F;
        #endregion

        #region Hoverfish
        [Slider("Hoverfish Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "HoverfishHP")]
        public float HoverfishHP = 1F;
        #endregion

        #region Jellyray
        [Slider("Jellyray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "JellyrayHP")]
        public float JellyrayHP = 1F;
        #endregion

        #region Lava Larva
        [Slider("Lava Larva Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "LavaLarvaHP")]
        public float LavaLarvaHP = 1F;
        #endregion

        #region Lava Lizard
        [Slider("Lava Lizard Health", Min = 1F, Max = 1F, DefaultValue = 200F, Step = 1F, Id = "LavaLizardHP")]
        public float LavaLizardHP = 1F;
        #endregion

        #region Magmarang
        [Slider("Magmarang Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "MagmarangHP")]
        public float MagmarangHP = 1F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "MesmerHP")]
        public float MesmerHP = 1F;
        #endregion

        #region Oculus
        [Slider("Oculus Health", Min = 1F, Max = 1F, DefaultValue = 20F, Step = 1F, Id = "OculusHP")]
        public float OculusHP = 1F;
        #endregion

        #region Peeper
        [Slider("Peeper Health", Min = 1F, Max = 1F, DefaultValue = 20F, Step = 1F, Id = "PeeperHP")]
        public float PeeperHP = 1F;
        #endregion

        #region Rabbit Ray
        [Slider("Rabbit Ray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "RabbitRayHP")]
        public float RabbitRayHP = 1F;
        #endregion

        #region Reaper Leviathan
        [Slider("Reaper Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 5000F, Step = 1F, Id = "ReaperLeviathanHP")]
        public float ReaperLeviathanHP = 1F;
        #endregion

        #region Red Eyeye
        [Slider("Red Eyeye Health", Min = 1F, Max = 1F, DefaultValue = 25F, Step = 1F, Id = "RedEyeyeHP")]
        public float RedEyeyeHP = 1F;
        #endregion

        #region Reginald
        [Slider("Reginald Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "ReginaldHP")]
        public float ReginaldHP = 1F;
        #endregion

        #region River Prowler
        [Slider("River Prowler Health", Min = 1F, Max = 1F, DefaultValue = 200F, Step = 1F, Id = "RiverProwlerHP")]
        public float RiverProwlerHP = 1F;
        #endregion

        #region Sand Shark
        [Slider("Sand Shark Health", Min = 1F, Max = 1F, DefaultValue = 250F, Step = 1F, Id = "SandSharkHP")]
        public float SandSharkHP = 1F;
        #endregion

        #region Sea Dragon Leviathan
        [Slider("Sea Dragon Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 5000F, Step = 1F, Id = "SeaDragonLeviathanHP")]
        public float SeaDragonLeviathanHP = 1F;
        #endregion

        #region Sea Treader Leviathan
        [Slider("Sea Treader Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 3000F, Step = 1F, Id = "SeaTreaderLeviathanHP")]
        public float SeaTreaderLeviathanHP = 1F;
        #endregion

        #region Shuttlebug
        [Slider("Shuttlebug Health", Min = 1F, Max = 1F, DefaultValue = 50F, Step = 1F, Id = "ShuttlebugHP")]
        public float ShuttlebugHP = 1F;
        #endregion

        #region Spadefish
        [Slider("Spadefish Health", Min = 1F, Max = 1F, DefaultValue = 30F, Step = 1F, Id = "SpadefishHP")]
        public float SpadefishHP = 1F;
        #endregion

        #region Spinefish
        [Slider("Spinefish Health", Min = 1F, Max = 1F, DefaultValue = 20F, Step = 1F, Id = "SpinefishHP")]
        public float SpinefishHP = 1F;
        #endregion

        #region Skyray
        [Slider("Skyray Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "SkyrayHP")]
        public float SkyrayHP = 1F;
        #endregion

        #region Stalker
        [Slider("Stalker Health", Min = 1F, Max = 1F, DefaultValue = 300F, Step = 1F, Id = "StalkerHP")]
        public float StalkerHP = 1F;
        #endregion

        #region Warper
        [Slider("Warper Health", Min = 1F, Max = 1F, DefaultValue = 100F, Step = 1F, Id = "WarperHP")]
        public float WarperHP = 1F;
        #endregion
    }
}
