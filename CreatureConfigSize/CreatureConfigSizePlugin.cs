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
        [Slider("Small Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Id = "SmallCreatureMinSize",
            Tooltip = "Minimum size modifier of small-size creatures. For example, a value of '0.5' would make the minimum size half as small as the base size.")]
        public float SmallCreatureMinSize = 1f;
        [Slider("Small Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Id = "SmallCreatureMaxSize",
            Tooltip = "Maximum size modifier of small-size creatures. For example, a value of '2' would make the maximum size twice as big as the base size.")]
        public float SmallCreatureMaxSize = 1f;
        [Slider("Medium Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Id = "MedCreatureMinSize",
            Tooltip = "Minimum size modifier of medium-size creatures. For example, a value of '0.5' would make the minimum size half as small as the base size.")]
        public float MedCreatureMinSize = 1f;
        [Slider("Medium Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Id = "MedCreatureMaxSize",
            Tooltip = "Minimum size modifier of medium-size creatures. For example, a value of '2' would make the maximum size twice as big as the base size.")]
        public float MedCreatureMaxSize = 1f;
        [Slider("Large Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Id = "LargeCreatureMinSize",
            Tooltip = "Minimum size modifier of leviathan-size creatures. For example, a value of '0.5' would make the minimum size half as small as the base size.")]
        public float LargeCreatureMinSize = 1f;
        [Slider("Large Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Id = "LargeCreatureMaxSize",
            Tooltip = "Maximum size modifier of leviathan-size creatures. For example, a value of '2' would make the maximum size twice as big as the base size.")]
        public float LargeCreatureMaxSize = 1f;
    }
}
