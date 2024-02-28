using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace CreatureConfigDamageLite
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class CreatureConfigDamageLitePlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfigdamagelite";
        private const string pluginName = "Creature Config - Damage (Lite)";
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

    [Menu("Creature Config - Damage (Lite)")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Slider("Damage Presets", Min = 2F, Max = 8F, DefaultValue = 5F, Step = 1F, Id = "DamagePreset", 
            Tooltip = "The damage multiplier preset you wish to use if you want to quickly change all damage values. \n" +
            "2 = Sandbox, all enemies deal 1 damage \n" +
            "3 = Very Easy, All enemies deal 50% less damage \n" +
            "4 = Easy, All enemies deal 25% less damage \n" +
            "5 = Default, All enemies deal default damage \n" +
            "6 = Hard, All enemies deal 25% more damage \n" +
            "7 = Very Hard, All enemies deal 50% more damage \n" +
            "8 = Sudden Death, All enemies will kill the player in one hit, including vehicles (cyclops takes 2 hits); you have been warned")]
        public float DamagePreset = 5.0F;
    }
}