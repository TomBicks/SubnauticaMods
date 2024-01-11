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
        #region Ampeel
        [Slider("Ampeel Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "AmpeelHP")]
        public float AmpeelHP = 1F;
        #endregion

        #region Biter
        [Slider("Biter Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "BiterHP")]
        public float BiterHP = 1F;
        #endregion

        #region Bleeder
        [Slider("Bleeder Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "BleederHP")]
        public float BleederHP = 1F;
        #endregion

        #region Blood Crawler
        [Slider("Blood Crawler Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "BloodCrawlerHP")]
        public float BloodCrawlerHP = 1F;
        #endregion

        #region Boneshark
        [Slider("Boneshark Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "BonesharkHP")]
        public float BonesharkHP = 1F;
        #endregion

        #region Cave Crawler
        [Slider("Cave Crawler Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "CaveCrawlerHP")]
        public float CaveCrawlerHP = 1F;
        #endregion

        #region Crabsnake
        [Slider("Crabsnake Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "CrabsnakeHP")]
        public float CrabsnakeHP = 1F;
        #endregion

        #region Crabsquid
        [Slider("Crabsquid Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "CrabsquidHP")]
        public float CrabsquidHP = 1F;
        #endregion

        #region Crashfish
        [Slider("Crashfish Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "CrashfishHP")]
        public float CrashfishHP = 1F;
        #endregion

        #region Gasopod
        [Slider("Gasopod Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "GasopodHP")]
        public float GasopodHP = 1F;
        #endregion

        #region Ghost Leviathan
        [Slider("Ghost Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "GhostLeviathanHP")]
        public float GhostLeviathanHP = 1F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Ghost Leviathan Juv. Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "GhostLeviathanJuvenileHP")]
        public float GhostLeviathanJuvenileHP = 1F;
        #endregion

        #region Lava Lizard
        [Slider("Lava Lizard Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "LavaLizardHP")]
        public float LavaLizardHP = 1F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "MesmerHP")]
        public float MesmerHP = 1F;
        #endregion

        #region Reaper Leviathan
        [Slider("Reaper Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "ReaperLeviathanHP")]
        public float ReaperLeviathanHP = 1F;
        #endregion

        #region River Prowler
        [Slider("River Prowler Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "RiverProwlerHP")]
        public float RiverProwlerHP = 1F;
        #endregion

        #region Sand Shark
        [Slider("Sand Shark Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "SandSharkHP")]
        public float SandSharkHP = 1F;
        #endregion

        #region Sea Dragon Leviathan
        [Slider("Sea Dragon Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "SeaDragonLeviathanHP")]
        public float SeaDragonLeviathanHP = 1F;
        #endregion

        #region Sea Treader Leviathan
        [Slider("Sea Treader Leviathan Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "SeaTreaderLeviathanHP")]
        public float SeaTreaderLeviathanHP = 1F;
        #endregion

        #region Stalker
        [Slider("Stalker Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "StalkerHP")]
        public float StalkerHP = 1F;
        #endregion

        #region Warper
        [Slider("Warper Health", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 1F, Id = "WarperHP")]
        public float WarperHP = 1F;
        #endregion
    }
}
