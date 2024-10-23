using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.Attributes;
using Nautilus.Json;
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
        private const string versionString = "0.8.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static ManualLogSource logger { get; private set; }

        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        //TODO!! Maybe think of a better, more fitting name?
        internal static CreatureSizeInfo creatureSizeInfo { get; } = SaveDataHandler.RegisterSaveDataCache<CreatureSizeInfo>();

        [FileName("creature_size_info")]
        internal class CreatureSizeInfo : SaveDataCache
        {
            public bool SizeRandomised { get; set; }
            //A list (with its own class, like I did with MoreLeviathanSpawns), that maybe lists size modifier, default size?
            //TODO!! Check QCreatureConfig for some inspiration for what might be useful to include
            //SizeChanged - Bool value as to whether this creature has had its size changed or not (no shouldn't need it; if it's in the list, its size has been changed!!)
            //TechType - Only set it once (or close as possible) and pull from the list whenever needing to refer to the creature and size
            //Size Modifier - Will be the size modifier set based on the techtype; won't need to rerandomise again, so this won't change and can be static?
            public List<float> CreatureInfo { get; set; }
        }

        private void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(CommandsSize));
        }
    }

    [Menu("Creature Config - Size")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Toggle("Allow all creatures to be picked up?",
            Tooltip = "Allow any fish, regardless of size, to be picked up. Without this enabled, only fish small enough can be picked up.")]
        public bool AllowAllPickupable = false;
        [Toggle("Allow all creatures to be placed into alien containment?",
            Tooltip = "Allow any fish, regardless of size, to be placed into alien containment. Be default, only fish that can physically fit in containment are allowed, so take care when enabling this.")]
        public bool AllowAllWaterPark = false;
        [Toggle("Enable complex size customisation?",
            Tooltip = "Enable complex changes to size range of each creature, which can be changed in the config file for this mod. If unchecked, will use the size range options below.")]
        public bool ComplexSizeEnabled = false;

        //NOTE!! May use these for the 'simple' option, where the user *doesn't* customise every single creature
        //TODO!! Make it clear it's a random valueb between the min amd max ranges
        [Slider("Small Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "SmallCreatureMinSize",
            Tooltip = "Minimum size modifier of small creatures. Will be randomly generated between this and the maximum size, then multiplied against the creature's base size.")]
        public float SmallCreatureMinSize = 1f;
        [Slider("Small Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "SmallCreatureMaxSize",
            Tooltip = "Maximum size modifier of small creatures. Will be randomly generated between this and the minimum size, then multiplied against the creature's base size.")]
        public float SmallCreatureMaxSize = 1f;
        [Slider("Medium Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "MedCreatureMinSize",
            Tooltip = "Minimum size modifier of medium creatures. Will be randomly generated between this and the maximum size, then multiplied against the creature's base size.")]
        public float MedCreatureMinSize = 1f;
        [Slider("Medium Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "MedCreatureMaxSize",
            Tooltip = "Maximum size modifier of medium creatures. Will be randomly generated between this and the minimum size, then multiplied against the creature's base size.")]
        public float MedCreatureMaxSize = 1f;
        [Slider("Large Creature Minimum Size", Min = 0.1f, Max = 1f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "LargeCreatureMinSize",
            Tooltip = "Minimum size modifier of large creatures. Will be randomly generated between this and the maximum size, then multiplied against the creature's base size.")]
        public float LargeCreatureMinSize = 1f;
        [Slider("Large Creature Maximum Size", Min = 1f, Max = 4f, DefaultValue = 1f, Step = 0.1f, Format = "{0:F1}", Id = "LargeCreatureMaxSize",
            Tooltip = "Maximum size modifier of large creatures. Will be randomly generated between this and the minimum size, then multiplied against the creature's base size.")]
        public float LargeCreatureMaxSize = 1f;

        //NOTE!! May use this for the 'complex' option, where the user *can* customise every single creature
        //NOTE!! Any changes made to this dictionary in the text file won't be changed when the config options in-game are changed
        //TODO!! Create a dictionary of min and max values, with assocaited techTypes, so the users can customise each and every value to their whim
        //TODO!! Utilise the dictionary, if the config is set, to bypass the three sizeclass system and directly change the min and max values
        //NOTE!! The default min and max here is 0.9 and 1.1, so that the decimals show up in the text file and the user is aware they can set them to decimals
        public Dictionary<TechType, (float min, float max)> CreatureSizeRangeReference = new Dictionary<TechType, (float, float)>()
        {
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
            { TechType.SeaEmperorBaby, (0.9f, 1.1f) },
            { TechType.SeaEmperorJuvenile , (0.9f, 1.1f) }, //TechType for the several Sea Emperors found in the world
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
