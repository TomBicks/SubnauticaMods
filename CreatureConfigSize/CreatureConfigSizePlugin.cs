using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace CreatureConfigSize
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    internal class CreatureConfigSizePlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfigsize";
        private const string pluginName = "Creature Config - Size";
        private const string versionString = "0.0.1";

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

    [Menu("Creature Config - Size")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "SmallCreatureMinSize",
            Tooltip = "Minimum size modifier of small-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float SmallCreatureMinSize = 1F;
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "SmallCreatureMaxSize",
            Tooltip = "Maximum size modifier of small-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float SmallCreatureMaxSize = 1F;
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "MedCreatureMinSize",
            Tooltip = "Minimum size modifier of medium-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float MedCreatureMinSize = 1F;
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "MedCreatureMaxSize",
            Tooltip = "Minimum size modifier of medium-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float MedCreatureMaxSize = 1F;
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "LargeCreatureMinSize",
            Tooltip = "Minimum size modifier of leviathan-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float LargeCreatureMinSize = 1F;
        [Slider("Small Creature Minimum Size", Min = 1F, Max = 1F, DefaultValue = 1F, Step = 0.1F, Id = "LargeCreatureMaxSize",
            Tooltip = "Maximum size modifier of leviathan-size creatures. For example, a value of '2' would make the minimum size two times as small as base.")]
        public float LargeCreatureMaxSize = 1F;
    }
}
