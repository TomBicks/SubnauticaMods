using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace ImmortalSnail
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    internal class ImmortalSnailPlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.immortalsnail";
        private const string pluginName = "Immortal Snail";
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

    [Menu("Immortal Snail")]
    public class Config : Nautilus.Json.ConfigFile
    {

    }
}
