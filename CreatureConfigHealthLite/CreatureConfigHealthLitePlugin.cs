using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace CreatureConfigHealthLite
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class CreatureConfigHealthLitePlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfighealthlite";
        private const string pluginName = "Creature Config - Health (Lite)";
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

    [Menu("Creature Config - Health (Lite)")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Toggle("Apply presets to aggressive fauna only?", Tooltip = "Apply presets to aggressive fauna only, so you can quickly apply sweeping changes without your thermoblade unable to kill a Peeper.")]
        public bool ApplyPresetsToAggressiveOnly = false;

        [Toggle("Make Cuddlefish invulnerable?", Tooltip = "Makes Cuddlefish invulnerable, so you don't have to worry about any danger to them.")]
        public bool CuddlefishInvunerable = false;

        [Toggle("Exclude changes to Bleeders?", Tooltip = "Excludes Bleeders' from health changes, as Bleeder's don't let go without being killed first or using the Propulsion or Repulsion Cannon.")]
        public bool ExcludeBleeder = false;

        [Slider("Health Presets", Min = 2F, Max = 8F, DefaultValue = 1F, Step = 1F, Id = "HealthPreset",
            Tooltip = "The health multiplier preset you wish to use if you want to quickly change all damage values. \n" +
            "2 = One-Hit, Set creatures have 1 health \n" +
            "3 = Very Easy, Set creatures have 50% less health \n" +
            "4 = Easy, Set creatures have 25% less health \n" +
            "5 = Default, Set creatures have default health \n" +
            "6 = Hard, Set creatures have 25% more health \n" +
            "7 = Very Hard, Set creatures heave 50% more health \n" +
            "8 = Invulnerable, Set creatures are invulnerable and cannot be damaged")]
        public float HealthPreset = 5.0F;
    }
}
