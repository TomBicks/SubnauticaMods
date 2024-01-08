using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;

namespace CreatureConfig
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class CreatureConfigPlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfig";
        private const string pluginName = "Creature Config";
        private const string versionString = "0.9.0";

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

    [Menu("Creature Config")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Slider("Damage Presets", Min = 1F, Max = 8F, DefaultValue = 5F, Step = 1F, Id = "DamagePreset", 
            Tooltip = "The damage multiplier preset you wish to use if you want to quickly change all damage values. \n" +
            "Keep in mind that changes made to individual creatures below will not take effect unless you select preset 1, Custom. \n" +
            "1 = Custom, any individual changes made below will take effect \n" +
            "2 = Sandbox, all enemies deal 1 damage \n" +
            "3 = Very Easy, All enemies deal 50% less damage \n" +
            "4 = Easy, All enemies deal 25% less damage \n" +
            "5 = Default, All enemies deal default damage \n" +
            "6 = Hard, All enemies deal 25% more damage \n" +
            "7 = Very Hard, All enemies deal 50% more damage \n" +
            "8 = Sudden Death, All enemies will kill the player in one hit, including vehicles (cyclops takes 2 hits); you have been warned")]
        public float DamagePreset = 1.0F;

        //NOTE!! Seamoth has 300 HP, Prawn Suit has 600 HP, and Cyclops has 1500 HP. As such, some attacks designed for vehicles may do less damage than appears

        #region Ampeel
        [Slider("Ampeel Bite Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "AmpeelBiteDmg", 
            Tooltip = "Damage dealt by an Ampeel's bite.")]
        public float AmpeelBiteDmg = 30F;
        [Slider("Ampeel Shock Damage", Min = 1F, Max = 100F, DefaultValue = 15F, Step = 1F, Id = "AmpeelShockDmg", 
            Tooltip = "Damage dealt by an Ampeel's electricity.")]
        public float AmpeelShockDmg = 15F;
        [Slider("Ampeel Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 50F, Step = 5F, Id = "AmpeelCyclopsDmg", 
            Tooltip = "Damage dealt by an Ampeel's bite to a Cyclops.")]
        public float AmpeelCyclopsDmg = 50F;
        #endregion

        #region Biter
        [Slider("Biter Damage", Min = 1F, Max = 100F, DefaultValue = 7F, Step = 1F, Id = "BiterDmg", 
            Tooltip = "Damage dealt by a Biter's bite.")]
        public float BiterDmg = 7F;
        #endregion

        #region Bleeder
        [Slider("Bleeder Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "BleederDmg", 
            Tooltip = "Damage dealt by a Bleeder's grab attack.")]
        public float BleederDmg = 5F;
        #endregion

        #region Blood Crawler
        [Slider("Blood Crawler Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "BloodCrawlerDmg", 
            Tooltip = "Damage dealt by a Blood Crawler's bite.")]
        public float BloodCrawlerDmg = 5F;
        #endregion

        #region Blighter
        [Slider("Blighter Damage", Min = 1F, Max = 100F, DefaultValue = 7F, Step = 1F, Id = "BlighterDmg", 
            Tooltip = "Damage dealt by a Blighter's bite.")]
        public float BlighterDmg = 7F;
        #endregion

        #region Boneshark
        [Slider("Boneshark Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "BonesharkDmg", 
            Tooltip = "Damage dealt by a Boneshark's bite.")]
        public float BonesharkDmg = 30F;
        #endregion

        #region Cave Crawler
        [Slider("Cave Crawler Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "CaveCrawlerDmg", 
            Tooltip = "Damage dealt by a Cave Crawler's bite.")]
        public float CaveCrawlerDmg = 5F;
        #endregion

        #region Crabsnake
        [Slider("Crabsnake Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeDmg", 
            Tooltip = "Damage dealt by a Crabsnake's bite.")]
        public float CrabsnakeDmg = 35F;
        #endregion

        #region Crabsquid
        [Slider("Crabsquid Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "CrabsquidDmg", 
            Tooltip = "Damage dealt by a Crabsquid's pincers.")]
        public float CrabsquidDmg = 40F;
        #endregion

        #region Crashfish
        [Slider("Crashfish Damage", Min = 1F, Max = 100F, DefaultValue = 50F, Step = 1F, Id = "CrashfishDmg", 
            Tooltip = "Damage dealt by a Crashfish's explosion.")]
        public float CrashfishDmg = 0F;
        #endregion

        #region Gasopod
        [Slider("Gasopod Gaspod Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "GasopodGasPodDmg", 
            Tooltip = "Damage dealt by a Gasopod's poison cloud.")]
        public float GasopodGasPodDmg = 10F;
        #endregion

        #region Ghost Leviathan
        [Slider("Ghost Leviathan Damage", Min = 1F, Max = 100F, DefaultValue = 85F, Step = 1F, Id = "GhostLeviathanDmg", 
            Tooltip = "Damage dealt by a Ghost Leviathan's bite.")]
        public float GhostLeviathanDmg = 85F;
        [Slider("Ghost Leviathan Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 250F, Step = 5F, Id = "GhostLeviathanCyclopsDmg", 
            Tooltip = "Damage dealt by a Ghost Leviathan's bite to a Cyclops.")]
        public float GhostLeviathanCyclopsDmg = 250F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Ghost Leviathan Juv. Damage", Min = 1F, Max = 100F, DefaultValue = 55F, Step = 1F, Id = "GhostLeviathanJuvenileDmg", 
            Tooltip = "Damage dealt by a Juvenile Ghost Leviathan's bite.")]
        public float GhostLeviathanJuvenileDmg = 55F;
        [Slider("Ghost Leviathan Juv. Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 220F, Step = 5F, Id = "GhostLeviathanJuvenileCyclopsDmg", 
            Tooltip = "Damage dealt by a Juvenile Ghost Leviathan's bite to a Cyclops.")]
        public float GhostLeviathanJuvenileCyclopsDmg = 220F;
        #endregion

        #region Lava Lizard
        [Slider("Lava Lizard Bite Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "LavaLizardBiteDmg", 
            Tooltip = "Damage dealt by a Lava Lizard's bite.")]
        public float LavaLizardBiteDmg = 30F;
        [Slider("Lava Lizard Lava Rock Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 15F, Step = 1F, Id = "LavaLizardLavaRockDmg", 
            Tooltip = "Damage dealt by a Lava Lizard's lava rock projectile.")]
        public float LavaLizardLavaRockDmg = 15F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "MesmerDmg", 
            Tooltip = "Damage dealt by a Mesmer's bite.")]
        public float MesmerDmg = 35F;
        #endregion

        #region Reaper Leviathan
        [Slider("Reaper Damage", Min = 1F, Max = 100F, DefaultValue = 80F, Step = 1F, Id = "ReaperDmg", 
            Tooltip = "Damage dealt by a Reaper Leviathan's bite.")]
        public float ReaperDmg = 80F;
        [Slider("Reaper Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 220F, Step = 5F, Id = "ReaperCyclopsDmg", 
            Tooltip = "Damage dealt by a Reaper Leviathan's bite to a Cyclops.")]
        public float ReaperCyclopsDmg = 220F;
        #endregion

        #region River Prowler
        [Slider("River Prowler Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "RiverProwlerDmg", 
            Tooltip = "Damage dealt by a River Prowler's bite.")]
        public float RiverProwlerDmg = 30F;
        #endregion

        #region Sand Shark
        [Slider("Sand Shark Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "SandSharkDmg", 
            Tooltip = "Damage dealt by a Sandshark's bite.")]
        public float SandSharkDmg = 30F;
        #endregion

        #region Sea Dragon Leviathan
        [Slider("Sea Dragon Bite Damage", Min = 5F, Max = 500F, DefaultValue = 300F, Step = 5F, Id = "SeaDragonBiteDmg", 
            Tooltip = "Damage dealt by a Sea Dragon Leviathan's bite.")]
        public float SeaDragonBiteDmg = 300F;
        [Slider("Sea Dragon Swat Damage", Min = 1F, Max = 100F, DefaultValue = 70F, Step = 1F, Id = "SeaDragonSwatDmg", 
            Tooltip = "Damage dealt by a Sea Dragon Leviathan's swat.")]
        public float SeaDragonSwatDmg = 70F;
        [Slider("Sea Dragon Shove Damage", Min = 5F, Max = 500F, DefaultValue = 250F, Step = 5F, Id = "SeaDragonShoveDmg", 
            Tooltip = "Damage dealt by a Sea Dragon Leviathan shoving a Cyclops.")]
        public float SeaDragonShoveDmg = 250F;
        [Slider("Sea Dragon Burning Chunk Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "SeaDragonBurningChunkDmg", 
            Tooltip = "Damage dealt by a Sea Dragon Leviathan's burning chunk projectile volley.")]
        public float SeaDragonBurningChunkDmg = 10F;
        [Slider("Sea Dragon Lava Meteor Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "SeaDragonLavaMeteorDmg", 
            Tooltip = "Damage dealt by a Sea Dragon Leviathan's lava meteor projectile.")]
        public float SeaDragonLavaMeteorDmg = 40F;
        #endregion 

        #region Sea Treader Leviathan
        [Slider("Sea Treader Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "SeaTreaderDmg", 
            Tooltip = "Damage dealt by a Sea Treader Leviathan's peck.")]
        public float SeaTreaderDmg = 40F;
        #endregion 

        #region Stalker
        [Slider("Stalker Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "StalkerDmg", 
            Tooltip = "Damage dealt by a Stalker's bite.")]
        public float StalkerDmg = 30F;
        #endregion

        #region Warper
        [Slider("Warper Claw Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "WarperClawDmg", 
            Tooltip = "Damage dealt by a Warper's claws. Note that the damage is multiplied by the player's infection level, capping at 4x.")]
        public float WarperClawDmg = 30F; //Damage increases based on infection level??? Starts at 23??? But biteDamge is 30??? LiveMixin with InfectionMixin to calcualte damage???
        [Slider("Warper Warp Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "WarperWarpDmg", 
            Tooltip = "Damage dealt by a Warper's teleport projectile.")]
        public float WarperWarpDmg = 10F;
        #endregion
    }
}