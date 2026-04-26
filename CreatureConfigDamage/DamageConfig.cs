using Nautilus.Options.Attributes;
using System.Collections.Generic;

namespace CreatureConfigDamage;

[Menu("Creature Config - Damage")]
public class DamageConfig : Nautilus.Json.ConfigFile
{
    [Slider("Damage Presets", Min = 1f, Max = 8f, DefaultValue = 1f, Step = 1f, Id = "DamagePreset",
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
    public float DamagePreset = 1.0f;

    //NOTE!! Seamoth has 300 HP, Prawn Suit has 600 HP, and Cyclops has 1500 HP. As such, some attacks designed for vehicles may do less damage than appears when compared to attacking players

    #region Creature Damage Sliders
    //NOTE!! Do any of these sliders actually need the ID variable to function? Am I just wasting space and code?
    //Isn't that what the variable below is for?
    #region Ampeel
    [Slider("Ampeel Bite Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "AmpeelBiteDmg",
        Tooltip = "Damage dealt by an Ampeel's bite.")]
    public float AmpeelBite = 30f;
    [Slider("Ampeel Shock Damage", Min = 1f, Max = 100f, DefaultValue = 15f, Step = 1f, Id = "AmpeelShockDmg",
        Tooltip = "Damage dealt by an Ampeel's electricity.")]
    public float AmpeelShock = 15f;
    [Slider("Ampeel Cyclops Damage", Min = 5f, Max = 500f, DefaultValue = 50f, Step = 5f, Id = "AmpeelCyclopsShock",
        Tooltip = "Damage dealt by an Ampeel's electricity to a Cyclops.")]
    public float AmpeelCyclopsShock = 50f;
    #endregion

    #region Biter
    [Slider("Biter Damage", Min = 1f, Max = 100f, DefaultValue = 7f, Step = 1f, Id = "BiterBite",
        Tooltip = "Damage dealt by a Biter's bite.")]
    public float BiterBite = 7f;
    #endregion

    #region Bleeder
    [Slider("Bleeder Damage", Min = 1f, Max = 100f, DefaultValue = 5f, Step = 1f, Id = "BleederSuck",
        Tooltip = "Damage dealt by a Bleeder's grab attack every few seconds.")]
    public float BleederSuck = 5f;
    #endregion

    #region Blighter
    [Slider("Blighter Damage", Min = 1f, Max = 100f, DefaultValue = 7f, Step = 1f, Id = "BlighterBite",
        Tooltip = "Damage dealt by a Blighter's bite.")]
    public float BlighterBite = 7f;
    #endregion

    #region Blood Crawler
    [Slider("Blood Crawler Damage", Min = 1f, Max = 100f, DefaultValue = 5f, Step = 1f, Id = "BloodCrawlerBite",
        Tooltip = "Damage dealt by a Blood Crawler's bite.")]
    public float BloodCrawlerBite = 5f;
    #endregion

    #region Boneshark
    [Slider("Boneshark Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "BoneSharkBite",
        Tooltip = "Damage dealt by a Boneshark's bite.")]
    public float BoneSharkBite = 30f;
    #endregion

    #region Cave Crawler
    [Slider("Cave Crawler Damage", Min = 1f, Max = 100f, DefaultValue = 5f, Step = 1f, Id = "CaveCrawlerBite",
        Tooltip = "Damage dealt by a Cave Crawler's bite.")]
    public float CaveCrawlerBite = 5f;
    #endregion

    #region Crabsnake
    [Slider("Crabsnake Damage", Min = 1f, Max = 100f, DefaultValue = 35f, Step = 1f, Id = "CrabsnakeBite",
        Tooltip = "Damage dealt by a Crabsnake's bite.")]
    public float CrabsnakeBite = 35f;
    #endregion

    #region Crabsquid
    [Slider("Crabsquid Damage", Min = 1f, Max = 100f, DefaultValue = 40f, Step = 1f, Id = "CrabSquidBite",
        Tooltip = "Damage dealt by a Crabsquid's pincers.")]
    public float CrabSquidBite = 40f;
    #endregion

    #region Crashfish
    [Slider("Crashfish Damage", Min = 1f, Max = 100f, DefaultValue = 50f, Step = 1f, Id = "CrashfishExplosion",
        Tooltip = "Damage dealt by a Crashfish's explosion.")]
    public float CrashfishExplosion = 50f;
    #endregion

    //TODO!! Not a creature, nor does it have a TechType, so it's difficult to retrieve and change at the moment
    /*#region Drooping Stinger
    [Slider("Drooping Stinger Damage", Min = 1f, Max = 100f, DefaultValue = 50f, Step = 1f, Id = "DroopingStingerDmg",
        Tooltip = "Damage dealt by a Drooping Stinger's venom over a few seconds.")]
    public float DroopingStingerDmg = 50f;
    #endregion*/

    #region Gasopod
    [Slider("Gasopod Gaspod Damage", Min = 1f, Max = 100f, DefaultValue = 10f, Step = 1f, Id = "GasPodPoison",
        Tooltip = "Damage dealt per second by a Gasopod's Gas Pod's poison cloud.")]
    public float GasPodPoison = 10f;
    #endregion

    #region Ghost Leviathan
    [Slider("Ghost Leviathan Damage", Min = 1f, Max = 100f, DefaultValue = 85f, Step = 1f, Id = "GhostBite",
        Tooltip = "Damage dealt by a Ghost Leviathan's bite.")]
    public float GhostBite = 85f;
    [Slider("Ghost Leviathan Cyclops Damage", Min = 5f, Max = 500f, DefaultValue = 250f, Step = 5f, Id = "GhostCyclopsBite",
        Tooltip = "Damage dealt by a Ghost Leviathan's bite to a Cyclops.")]
    public float GhostCyclopsBite = 250f;
    #endregion

    #region Ghost Leviathan Juvenile
    [Slider("Ghost Leviathan Juv. Damage", Min = 1f, Max = 100f, DefaultValue = 55f, Step = 1f, Id = "GhostJuvBite",
        Tooltip = "Damage dealt by a Juvenile Ghost Leviathan's bite.")]
    public float GhostJuvBite = 55f;
    [Slider("Ghost Leviathan Juv. Cyclops Damage", Min = 5f, Max = 500f, DefaultValue = 220f, Step = 5f, Id = "GhostJuvCyclopsBite",
        Tooltip = "Damage dealt by a Juvenile Ghost Leviathan's bite to a Cyclops.")]
    public float GhostJuvCyclopsBite = 220f;
    #endregion

    #region Lava Lizard
    [Slider("Lava Lizard Bite Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "LavaLizardBite",
        Tooltip = "Damage dealt by a Lava Lizard's bite.")]
    public float LavaLizardBite = 30f;
    [Slider("Lava Lizard Lava Rock Damage", Min = 1f, Max = 100f, DefaultValue = 15f, Step = 1f, Id = "LavaLizardLavaRock",
        Tooltip = "Damage dealt by a Lava Lizard's lava rock projectile.")]
    public float LavaLizardLavaRock = 15f;
    #endregion

    #region Mesmer
    [Slider("Mesmer Damage", Min = 1f, Max = 100f, DefaultValue = 35f, Step = 1f, Id = "MesmerBite",
        Tooltip = "Damage dealt by a Mesmer's bite.")]
    public float MesmerBite = 35f;
    #endregion

    #region Reaper Leviathan
    [Slider("Reaper Damage", Min = 1f, Max = 100f, DefaultValue = 80f, Step = 1f, Id = "ReaperBite",
        Tooltip = "Damage dealt by a Reaper Leviathan's bite.")]
    public float ReaperBite = 80f;
    [Slider("Reaper Cyclops Damage", Min = 5f, Max = 500f, DefaultValue = 220f, Step = 5f, Id = "ReaperCyclopsBite",
        Tooltip = "Damage dealt by a Reaper Leviathan's bite to a Cyclops.")]
    public float ReaperCyclopsBite = 220f;
    #endregion

    #region River Prowler
    [Slider("River Prowler Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "RiverProwlerBite",
        Tooltip = "Damage dealt by a River Prowler's bite.")]
    public float RiverProwlerBite = 30f;
    #endregion

    #region Sand Shark
    [Slider("Sandshark Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "SandsharkBite",
        Tooltip = "Damage dealt by a Sandshark's bite.")]
    public float SandsharkBite = 30f;
    #endregion

    #region Sea Dragon Leviathan
    [Slider("Sea Dragon Bite Damage", Min = 5f, Max = 500f, DefaultValue = 300f, Step = 5f, Id = "SeaDragonBite",
        Tooltip = "Damage dealt by a Sea Dragon Leviathan's bite.")]
    public float SeaDragonBite = 300f;
    [Slider("Sea Dragon Swat Damage", Min = 1f, Max = 100f, DefaultValue = 70f, Step = 1f, Id = "SeaDragonSwat",
        Tooltip = "Damage dealt by a Sea Dragon Leviathan's swat.")]
    public float SeaDragonSwat = 70f;
    [Slider("Sea Dragon Shove Damage", Min = 5f, Max = 500f, DefaultValue = 250f, Step = 5f, Id = "SeaDragonShove",
        Tooltip = "Damage dealt by a Sea Dragon Leviathan shoving a Cyclops.")]
    public float SeaDragonShove = 250f;
    [Slider("Sea Dragon Burning Chunk Damage", Min = 1f, Max = 100f, DefaultValue = 10f, Step = 1f, Id = "SeaDragonBurningChunk",
        Tooltip = "Damage dealt by a Sea Dragon Leviathan's burning chunk projectile volley.")]
    public float SeaDragonBurningChunk = 10f;
    [Slider("Sea Dragon Lava Meteor Damage", Min = 1f, Max = 100f, DefaultValue = 40f, Step = 1f, Id = "SeaDragonLavaMeteor",
        Tooltip = "Damage dealt by a Sea Dragon Leviathan's lava meteor projectile.")]
    public float SeaDragonLavaMeteor = 40f;
    #endregion

    #region Sea Treader Leviathan
    [Slider("Sea Treader Damage", Min = 1f, Max = 100f, DefaultValue = 40f, Step = 1f, Id = "SeaTreaderPeck",
        Tooltip = "Damage dealt by a Sea Treader Leviathan's peck.")]
    public float SeaTreaderPeck = 40f;
    #endregion

    #region Stalker
    [Slider("Stalker Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "StalkerBite",
        Tooltip = "Damage dealt by a Stalker's bite.")]
    public float StalkerBite = 30f;
    #endregion

    #region Tiger Plant
    [Slider("Tiger Plant Damage", Min = 1f, Max = 100f, DefaultValue = 10f, Step = 1f, Id = "TigerPlantThorn",
        Tooltip = "Damage dealt by a Tiger Plant's thorn projectile.")]
    public float TigerPlantThorn = 10f;
    #endregion

    #region Warper
    [Slider("Warper Claw Damage", Min = 1f, Max = 100f, DefaultValue = 30f, Step = 1f, Id = "WarperClaw",
        Tooltip = "Damage dealt by a Warper's claws. Note that the damage is multiplied by the player's infection level, capping at 4x.")]
    public float WarperClaw = 30f; //TODO!!Damage increases based on infection level??? Starts at 23??? But biteDamge is 30??? LiveMixin with InfectionMixin to calcualte damage???
    [Slider("Warper Warp Damage", Min = 1f, Max = 100f, DefaultValue = 10f, Step = 1f, Id = "WarperWarpBall",
        Tooltip = "Damage dealt by a Warper's teleport projectile.")]
    public float WarperWarpBall = 10f;
    #endregion
    #endregion

    public Dictionary<TechType, bool> IgnoreArmour = new Dictionary<TechType, bool>()
        {
            { TechType.Shocker, false }, //TechType for Ampeel
            { TechType.Biter, false },
            { TechType.Bladderfish, false },
            { TechType.Bleeder, false },
            { TechType.Blighter, false },
            { TechType.Shuttlebug, false }, //TechType for Blood Crawler
            { TechType.BoneShark, false },
            { TechType.Boomerang, false },
            { TechType.CaveCrawler, false },
            { TechType.Crabsnake, false },
            { TechType.CrabSquid, false },
            { TechType.Crash, false }, //TechType for Crashfish
            //{ TechType.Gasopod, false }, //TODO!! Doesn't work; No TechType for 'dealer' when damaged by the gas
            { TechType.GhostLeviathan, false },
            { TechType.GhostLeviathanJuvenile, false },
            { TechType.LavaLizard, false },
            { TechType.Mesmer, false },
            { TechType.ReaperLeviathan, false },
            { TechType.SpineEel, false }, //TechType for River Prowler
            { TechType.Sandshark, false },
            { TechType.SeaDragon, false },
            { TechType.SeaTreader, false },
            { TechType.Stalker, false },
            { TechType.SpikePlant, false }, //TODO!! Doesn't work; No TechType for 'dealer' when shot with this projectile
            { TechType.Warper, false }
        };
}