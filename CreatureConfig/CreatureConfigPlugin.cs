using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Options.Attributes;
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
    public class Config : SMLHelper.V2.Json.ConfigFile
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

        #region Ampeel
        [Slider("Ampeel Bite Damage", Min = 1F, Max = 100F, DefaultValue = 45F, Step = 1F, Id = "AmpeelBiteDmg"), OnChange(nameof(DamageChanged))]
        public float AmpeelBiteDmg = 45F;
        [Slider("Ampeel Shock Damage", Min = 1F, Max = 100F, DefaultValue = 15F, Step = 1F, Id = "AmpeelShockDmg"), OnChange(nameof(DamageChanged))]
        public float AmpeelShockDmg = 15F;
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
        [Slider("Crabsnake Damage", Min = 1F, Max = 100F, DefaultValue = 5F, Step = 1F, Id = "BloodCrawlerDmg"), OnChange(nameof(DamageChanged))]
        public float BloodCrawlerDmg = 5F;
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

        /*#region Ghost Leviathan
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(SpawnIntensityChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion

        #region Ghost Leviathan Juvenile
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(SpawnIntensityChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion*/

        #region Lava Lizard
        [Slider("Lava Lizard Bite Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "LavaLizardBiteDmg"), OnChange(nameof(DamageChanged))]
        public float LavaLizardBiteDmg = 30F;
        [Slider("Lava Lizard Spit Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "LavaLizardSpitDmg"), OnChange(nameof(DamageChanged))]
        public float LavaLizardSpitDmg = 30F;
        #endregion

        #region Mesmer
        [Slider("Mesmer Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "MesmerDmg"), OnChange(nameof(DamageChanged))]
        public float MesmerDmg = 35F;
        #endregion

        /*#region Reaper Leviathan
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(SpawnIntensityChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion*/

        /*#region River Prowler
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(DamageChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion*/

        #region Sand Shark
        [Slider("Sand Shark Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "SandSharkDmg"), OnChange(nameof(DamageChanged))]
        public float SandSharkDmg = 30F;
        #endregion

        /*#region Sea Dragon Leviathan
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(SpawnIntensityChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion*/

        #region Stalker
        [Slider("Stalker Damage", Min = 1F, Max = 100F, DefaultValue = 30F, Step = 1F, Id = "StalkerDmg"), OnChange(nameof(DamageChanged))]
        public float StalkerDmg = 30F;
        #endregion

        /*#region Warper
        [Slider("Crabsnake Grab Damage", Min = 1F, Max = 100F, DefaultValue = 35F, Step = 1F, Id = "CrabsnakeGrabDmg"), OnChange(nameof(SpawnIntensityChanged))]
        public float CrabsnakeGrabDmg = 35F;
        #endregion*/

        private void PresetChanged(SliderChangedEventArgs e)
        {
            //float __percent = e.Value / 100;
            //BiterDmg = __percent * 7;
            //CrabsnakeDmg = __percente * 35;

            //THIS SHOULD ROUND AFTER THE PERCENTAGE MULTIPLICATION AND ROUND TO THE NEAREST INTEGER, RATHER THAN MULTIPLE BY 1 or 2 ALWAYS
            //BiterDmg = (int)(e.Value * 7);
            //CrabsnakeDmg = (int)(e.Value * 35);

            //What I think I'm going to do is have this slider do nothing on its own, but instead in the patched method to actually change the values, it will check the preset
            //if the preset is 4, it uses individual; if not, it goes through each creature and multiplies its damage by the given multiplication (so passing 7 * 0.5 for example)
            //Need a list of default values for this to work though, so going to use a Dictionary, with the specific default value it's looking for being a string key in the dictionary
            //For example, if we're looking for BonesharkDmg, the dictionary would have "BonesharkDmg" as a key athat would lead to a default value of 30
        }

        private void DamageChanged(SliderChangedEventArgs e)
        {
            switch(e.Id)
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
            }
        }
    }
}