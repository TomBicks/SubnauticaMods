using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Commands;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace ImmortalSnail
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class ImmortalSnailPlugin : BaseUnityPlugin
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

            //!!ERROR This isn't running AT ALL for some reason?
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(TestCommands));
            ErrorMessage.AddMessage("Commands registered...hopefully");
            logger.LogDebug("Commands registered...hopefully");
        }

        public static class TestCommands
        {
            [ConsoleCommand("testshake")]
            public static void TestShake()
            {
                //NOTE!! Works perfectly!
                ErrorMessage.AddMessage("Testing shake FX");
                logger.LogDebug("Testing shake FX");
                MainCameraControl.main.ShakeCamera(4f, 8f, MainCameraControl.ShakeMode.Quadratic, 1.2f);
            }
            [ConsoleCommand("testboom")]
            public static void TestExplosion()
            {
                //NOTE!! Doesn't appear to do anything
                ErrorMessage.AddMessage("Testing explosion FX");
                logger.LogDebug("Testing explosion FX");
                WorldForces.AddExplosion(new Vector3(0f, 0f, 0f), (double)new Utils.ScalarMonitor(0f).Get(), 8f, 5000f);
                //This is the actual damage code; needs to have a reference to the snail's position and gameObject
                //DamageSystem.RadiusDamage(2000f, base.transform.position, 500f, DamageType.Explosive, base.gameObject);
            }
        }
    }

    [Menu("Immortal Snail")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Toggle("Enable Bomb?", Tooltip = "Whether the bomb will be spawned in the world at all.")]
        public bool EnableMod = true;
    }
}
