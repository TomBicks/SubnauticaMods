using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using System.Collections.Generic;

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
        //NOTE!! May use these for the 'simple' option, where the user *doesn't* customise every single creature
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

        //NOTE!! May use this for the 'complex' option, where the user *can* customise every single creature
        //NOTE!! Any changes made to this dictionary in the text file won't be changed when the config options in-game are changed
        //TODO!! Create a dictionary of min and max values, with assocaited techTypes, so the users can customise each and every value to their whim
        //TODO!! Utilise the dictionary, if the config is set, to bypass the three sizeclass system and directly change the min and max values
        //NOTE!! The default min and max here is 0.9 and 1.1, so that the decimals show up in the text file and the user is aware they can set them to decimals
        public Dictionary<TechType, (float, float)> sizeReference = new Dictionary<TechType, (float, float)>()
        {
            { TechType.Biter, (0.9f,1.1f) },
            { TechType.Biter, (0.9f,1.1f) },
            { TechType.Bladderfish, (0.9f,1.1f) },
            { TechType.Bleeder, (0.9f,1.1f) },
            { TechType.Blighter, (0.9f,1.1f) },
            { TechType.BoneShark, (0.9f,1.1f) },
            { TechType.Boomerang, (0.9f,1.1f) },
            { TechType.CaveCrawler, (0.9f,1.1f) },
            { TechType.Crabsnake, (0.9f,1.1f) },
            { TechType.CrabSquid, (0.9f,1.1f) },
            { TechType.Crash, (0.9f,1.1f) }, //TechType for Crashfish
            { TechType.Cutefish, (0.9f,1.1f) }, //TechType for Cuddlefish
            { TechType.Eyeye, (0.9f,1.1f) },
            { TechType.Floater,(0.9f, 1.1f) },
            { TechType.GarryFish, (0.9f,1.1f) },
            { TechType.Gasopod, (0.9f,1.1f) },
            { TechType.GhostLeviathan, (0.9f,1.1f) },
            { TechType.GhostLeviathanJuvenile, (0.9f,1.1f) },
            { TechType.GhostRayBlue, (0.9f,1.1f) }, //TechType for Ghost Ray
            { TechType.GhostRayRed,(0.9f, 1.1f) }, //TechType for Crimson Ray
            { TechType.HoleFish,(0.9f, 1.1f) },
            { TechType.Hoopfish, (0.9f,1.1f) },
            { TechType.Hoverfish,(0.9f, 1.1f) },
            { TechType.Jellyray, (0.9f,1.1f) },
            { TechType.Jumper, (0.9f,1.1f) }, //TechType for Shuttlebug
            { TechType.LavaBoomerang,(0.9f, 1.1f) }, //TechType for Magmarang
            { TechType.LavaEyeye, (0.9f,1.1f) }, //TechType for Red Eyeye
            { TechType.LavaLarva, (0.9f,1.1f) },
            { TechType.LavaLizard, (0.9f,1.1f) },
            { TechType.Mesmer, (0.9f,1.1f) },
            { TechType.Oculus, (0.9f,1.1f) },
            { TechType.Peeper, (0.9f,1.1f) },
            { TechType.RabbitRay, (0.9f,1.1f) },
            { TechType.ReaperLeviathan, (0.9f,1.1f) },
            { TechType.Reginald, (0.9f, 1.1f) },
            { TechType.Sandshark, (0.9f,1.1f) },
            { TechType.SeaDragon, (0.9f,1.1f) },
            { TechType.SeaTreader, (0.9f,1.1f) },
            { TechType.Shocker, (0.9f,1.1f) }, //TechType for Ampeel
            { TechType.Shuttlebug, (0.9f,1.1f) }, //TechType for Blood Crawler
            { TechType.Skyray, (0.9f,1.1f) },
            { TechType.Spadefish, (0.9f,1.1f) },
            { TechType.SpineEel, (0.9f,1.1f) }, //TechType for River Prowler
            { TechType.Spinefish, (0.9f,1.1f) },
            { TechType.Stalker, (0.9f,1.1f) },
            { TechType.Warper, (0.9f,1.1f) }
        };
    }
}
