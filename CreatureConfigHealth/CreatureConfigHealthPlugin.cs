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
        
    }
}
