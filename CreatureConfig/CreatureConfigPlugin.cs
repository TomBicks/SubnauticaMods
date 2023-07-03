using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;
using static OVRHaptics;

namespace CreatureConfig
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class CreatureConfigPlugin : BaseUnityPlugin
    {
        private const string myGUID = "com.jukebox.creatureconfig";
        private const string pluginName = "Creature Config";
        private const string versionString = "0.0.1";

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
            "3 = Very Easy, All enemies deal 50% damage \n" +
            "4 = Easy, All enemies deal 75% damage \n" +
            "5 = Default, All enemies deal 100% damage \n" +
            "6 = Hard, All enemies deal 125% damage \n" +
            "7 = Very Hard, All enemies deal 150% damage \n" +
            "8 = Sudden Death, All enemies will kill the player in one hit, including vehicles; you have been warned"), 
            OnChange(nameof(PresetChanged))]
        public float DamagePreset = 1.0F;

        //Note!! Seamoth has 200 HP, Prawn Suit has 600 HP, and Cyclops has 1500 HP. As such, some attacks designed for vehicles may do less damage than appears
        //Might be good to put this blurb on the modpage

        #region Ampeel
        [Slider("Ampeel Bite Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "AmpeelBiteDmg"), OnChange(nameof(DamageChanged))]
        public float AmpeelBiteDmg = 30F;
        [Slider("Ampeel Shock Damage", Min = 1F, Max = 100F, DefaultValue = 15F, Step = 1F, Id = "AmpeelShockDmg"), OnChange(nameof(DamageChanged))]
        public float AmpeelShockDmg = 15F;
        [Slider("Ampeel Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 50F, Step = 5F, Id = "AmpeelCyclopsDmg"), OnChange(nameof(DamageChanged))]
        public float AmpeelCyclopsDmg = 50F;
        #endregion

        #region Biter
        [Slider("Biter Damage", Min = 1F, Max = 100F, DefaultValue = 7F, Step = 1F, Id = "BiterDmg"), OnChange(nameof(DamageChanged))]
        public float BiterDmg = 7F;
        #endregion

        #region Bleeder
        [Slider("Bleeder Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "BleederDmg"), OnChange(nameof(DamageChanged))]
        public float BleederDmg = 5F;
        #endregion

        #region Blood Crawler
        [Slider("Blood Crawler Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "BloodCrawlerDmg"), OnChange(nameof(DamageChanged))]
        public float BloodCrawlerDmg = 5F;
        #endregion

        #region Blighter
        [Slider("Blighter Damage", Min = 1F, Max = 100F, DefaultValue = 7F, Step = 1F, Id = "BlighterDmg"), OnChange(nameof(DamageChanged))]
        public float BlighterDmg = 7F;
        #endregion

        #region Boneshark
        [Slider("Boneshark Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "BonesharkDmg"), OnChange(nameof(DamageChanged))]
        public float BonesharkDmg = 30F;
        #endregion

        #region Cave Crawler
        [Slider("Cave Crawler Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "CaveCrawlerDmg"), OnChange(nameof(DamageChanged))]
        public float CaveCrawlerDmg = 5F;
        #endregion

        #region Crabsnake
        [Slider("Crabsnake Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeDmg"), OnChange(nameof(DamageChanged))]
        public float CrabsnakeDmg = 35F;
        #endregion

        #region Crabsquid
        [Slider("Crabsquid Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "CrabsquidDmg"), OnChange(nameof(DamageChanged))]
        public float CrabsquidDmg = 40F;
        #endregion

        #region Crashfish
        [Slider("Crashfish Damage", Min = 1F, Max = 100F, DefaultValue = 50F, Step = 1F, Id = "CrashfishDmg"), OnChange(nameof(DamageChanged))]
        public float CrashfishDmg = 0F;
        #endregion

        #region Gasopod
        [Slider("Gasopod Gaspod Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "GasopodGasPodDmg"), OnChange(nameof(DamageChanged))]
        public float GasopodGasPodDmg = 10F;
        #endregion

        #region Ghost Leviathan
        [Slider("Ghost Leviathan Damage", Min = 1F, Max = 100F, DefaultValue = 85F, Step = 1F, Id = "GhostLeviathanDmg"), OnChange(nameof(DamageChanged))]
        public float GhostLeviathanDmg = 85F;
        [Slider("Ghost Leviathan Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 250F, Step = 5F, Id = "GhostLeviathanCyclopsDmg"), OnChange(nameof(DamageChanged))]
        public float GhostLeviathanCyclopsDmg = 250F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Ghost Leviathan Juvenile Damage", Min = 1F, Max = 100F, DefaultValue = 55F, Step = 1F, Id = "GhostLeviathanJuvenileDmg"), OnChange(nameof(DamageChanged))]
        public float GhostLeviathanJuvenileDmg = 55F;
        [Slider("Ghost Leviathan Juvenile Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 220F, Step = 5F, Id = "GhostLeviathanJuvenileCyclopsDmg"), OnChange(nameof(DamageChanged))]
        public float GhostLeviathanJuvenileCyclopsDmg = 220F;
        #endregion

        #region Lava Lizard
        [Slider("Lava Lizard Bite Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "LavaLizardBiteDmg"), OnChange(nameof(DamageChanged))]
        public float LavaLizardBiteDmg = 30F;
        [Slider("Lava Lizard Lava Rock Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 15F, Step = 1F, Id = "LavaLizardLavaRockDmg"), OnChange(nameof(DamageChanged))]
        public float LavaLizardLavaRockDmg = 15F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "MesmerDmg"), OnChange(nameof(DamageChanged))]
        public float MesmerDmg = 35F;
        #endregion

        #region Reaper Leviathan
        [Slider("Reaper Damage", Min = 1F, Max = 100F, DefaultValue = 80F, Step = 1F, Id = "ReaperDmg", Tooltip = "Damage dealt by a reaper to the player, seamoth & prawn suit"), OnChange(nameof(DamageChanged))]
        public float ReaperDmg = 80F;
        [Slider("Reaper Cyclops Damage", Min = 5F, Max = 500F, DefaultValue = 220F, Step = 5F, Id = "ReaperCyclopsDmg", Tooltip = "Damage dealt by a reaper to the cyclops"), OnChange(nameof(DamageChanged))]
        public float ReaperCyclopsDmg = 220F;
        #endregion

        #region River Prowler
        [Slider("River Prowler Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "RiverProwlerDmg"), OnChange(nameof(DamageChanged))]
        public float RiverProwlerDmg = 30F;
        #endregion

        #region Sand Shark
        [Slider("Sand Shark Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "SandSharkDmg"), OnChange(nameof(DamageChanged))]
        public float SandSharkDmg = 30F;
        #endregion

        #region Sea Dragon Leviathan
        [Slider("Sea Dragon Bite Damage", Min = 5F, Max = 500F, DefaultValue = 300F, Step = 5F, Id = "SeaDragonBiteDmg"), OnChange(nameof(DamageChanged))]
        public float SeaDragonBiteDmg = 300F;
        [Slider("Sea Dragon Swat Damage", Min = 1F, Max = 100F, DefaultValue = 70F, Step = 1F, Id = "SeaDragonSwatDmg"), OnChange(nameof(DamageChanged))]
        public float SeaDragonSwatDmg = 70F;
        [Slider("Sea Dragon Shove Damage", Min = 5F, Max = 500F, DefaultValue = 250F, Step = 5F, Id = "SeaDragonShoveDmg"), OnChange(nameof(DamageChanged))]
        public float SeaDragonShoveDmg = 250F;
        [Slider("Sea Dragon Burning Chunk Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "SeaDragonBurningChunkDmg"), OnChange(nameof(DamageChanged))]
        public float SeaDragonBurningChunkDmg = 10F;
        [Slider("Sea Dragon Lava Meteor Projectile Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "SeaDragonLavaMeteorDmg"), OnChange(nameof(DamageChanged))]
        public float SeaDragonLavaMeteorDmg = 40F;
        #endregion 

        #region Sea Treader Leviathan
        [Slider("Sea Treader Damage", Min = 1F, Max = 100F, DefaultValue = 40F, Step = 1F, Id = "SeaTreaderDmg"), OnChange(nameof(DamageChanged))]
        public float SeaTreaderDmg = 40F;
        #endregion 

        #region Stalker
        [Slider("Stalker Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "StalkerDmg"), OnChange(nameof(DamageChanged))]
        public float StalkerDmg = 30F;
        #endregion

        #region Warper
        [Slider("Warper Claw Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "WarperClawDmg", Tooltip = "Damage dealt by a warper's claws. Note that the damage is multiplied by the player's infection level, capping at 4x."), OnChange(nameof(DamageChanged))]
        public float WarperClawDmg = 30F; //Damage increases based on infection level??? Starts at 23??? But biteDamge is 30??? LiveMixin with InfectionMixin to calcualte damage???
        [Slider("Warper Warp Damage", Min = 1F, Max = 100F, DefaultValue = 10F, Step = 1F, Id = "WarperWarpDmg"), OnChange(nameof(DamageChanged))]
        public float WarperWarpDmg = 10F;
        #endregion

        private void PresetChanged(SliderChangedEventArgs e)
        {
            //DamagePreset = e.Value;
        }

        private void DamageChanged(SliderChangedEventArgs e)
        {
            /*switch(e.Id)
            {
                case "BiterDmg":
                    BiterDmg = e.Value;
                    break;
                case "BonesharkDmg":
                    BonesharkDmg = e.Value;
                    break;
                case "CrabsnakeDmg":
                    CrabsnakeDmg = e.Value;
                    break;
            }*/
        }
    }
}