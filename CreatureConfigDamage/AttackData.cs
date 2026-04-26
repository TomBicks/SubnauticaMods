using System;
using System.Collections.Generic;

namespace CreatureConfigDamage;

internal class AttackData
{
    public static readonly List<(TechType, AttackInfo[])> creatureAttacks = new List<(TechType, AttackInfo[])>()
    {
        { (TechType.Shocker, new AttackInfo[] { //Ampeel
            new AttackInfo { attackKey="AmpeelBite", defaultDamage=30f, configValue=() => Plugin.config.AmpeelBite },
            new AttackInfo { attackKey="AmpeelShock", defaultDamage=15f, configValue=() => Plugin.config.AmpeelShock, isGenericAttack=false },
            new AttackInfo { attackKey="AmpeelCyclopsShock", defaultDamage=50f, configValue=() => Plugin.config.AmpeelCyclopsShock, isGenericAttack=false }
        })},
        { (TechType.Biter, new AttackInfo[] {
            new AttackInfo { attackKey="BiterBite", defaultDamage=7f, configValue=() => Plugin.config.BiterBite }
        })},
        { (TechType.Bleeder, new AttackInfo[] {
            new AttackInfo { attackKey="BleederSuck", defaultDamage=5f, configValue=() => Plugin.config.BleederSuck, isGenericAttack=false }
        })},
        { (TechType.Blighter, new AttackInfo[] {
            new AttackInfo { attackKey="BlighterBite", defaultDamage=7f, configValue=() => Plugin.config.BlighterBite }
        })},
        { (TechType.Shuttlebug, new AttackInfo[] { //Blood Crawler
            new AttackInfo { attackKey="BloodCrawlerBite", defaultDamage=5f, configValue=() => Plugin.config.BloodCrawlerBite }
        })},
        { (TechType.BoneShark, new AttackInfo[] {
            new AttackInfo { attackKey="BoneSharkBite", defaultDamage=30f, configValue=() => Plugin.config.BoneSharkBite }
        })},
        { (TechType.CaveCrawler, new AttackInfo[] {
            new AttackInfo { attackKey="CaveCrawlerBite", defaultDamage=5f, configValue=() => Plugin.config.CaveCrawlerBite }
        })},
        { (TechType.Crabsnake, new AttackInfo[] {
            new AttackInfo { attackKey="CrabsnakeBite", defaultDamage=35f, configValue=() => Plugin.config.CrabsnakeBite, isGenericAttack=false }
        })},
        { (TechType.CrabSquid, new AttackInfo[] {
            new AttackInfo { attackKey="CrabSquidBite", defaultDamage=40f, configValue=() => Plugin.config.CrabSquidBite }
        })},
        { (TechType.Crash, new AttackInfo[] { //Crashfish
            new AttackInfo { attackKey="CrashfishExplosion", defaultDamage=50f, configValue=() => Plugin.config.CrashfishExplosion, isGenericAttack=false }
        })},
        { (TechType.GasPod, new AttackInfo[] { //Gasopod's Gas Pods (the damage is tied to the pods, not the creature itself)
            new AttackInfo { attackKey="GasPodPoison", defaultDamage=10f, configValue=() => Plugin.config.GasPodPoison, isGenericAttack=false }
        })},
        { (TechType.GhostLeviathan, new AttackInfo[] {
            new AttackInfo { attackKey="GhostBite", defaultDamage=85f, configValue=() => Plugin.config.GhostBite, isGenericAttack=false },
            new AttackInfo { attackKey="GhostCyclopsBite", defaultDamage=250f, configValue=() => Plugin.config.GhostCyclopsBite, isGenericAttack=false }
        })},
        { (TechType.GhostLeviathanJuvenile, new AttackInfo[] {
            new AttackInfo { attackKey="GhostJuvBite", defaultDamage=55f, configValue=() => Plugin.config.GhostJuvBite, isGenericAttack=false },
            new AttackInfo { attackKey="GhostJuvCyclopsBite", defaultDamage=220f, configValue=() => Plugin.config.GhostJuvCyclopsBite, isGenericAttack=false }
        })},
        { (TechType.LavaLizard, new AttackInfo[] {
            new AttackInfo { attackKey="LavaLizardBite", defaultDamage=30f, configValue=() => Plugin.config.LavaLizardBite },
            new AttackInfo { attackKey="LavaLizardLavaRock", defaultDamage=15f, configValue=() => Plugin.config.LavaLizardLavaRock, isGenericAttack=false }
        })},
        { (TechType.Mesmer, new AttackInfo[] {
            new AttackInfo { attackKey="MesmerBite", defaultDamage=35f, configValue=() => Plugin.config.MesmerBite }
        })},
        { (TechType.ReaperLeviathan, new AttackInfo[] {
            new AttackInfo { attackKey="ReaperBite", defaultDamage=80f, configValue=() => Plugin.config.ReaperBite, isGenericAttack=false },
            new AttackInfo { attackKey="ReaperCyclopsBite", defaultDamage=220f, configValue=() => Plugin.config.ReaperCyclopsBite, isGenericAttack=false }
        })},
        { (TechType.SpineEel, new AttackInfo[] { //River Prowler
            new AttackInfo { attackKey="RiverProwlerBite", defaultDamage=30f, configValue=() => Plugin.config.RiverProwlerBite }
        })},
        { (TechType.Sandshark, new AttackInfo[] {
            new AttackInfo { attackKey="SandsharkBite", defaultDamage=30f, configValue=() => Plugin.config.SandsharkBite }
        })},
        { (TechType.SeaDragon, new AttackInfo[] { //TODO!! SeaDragon was accordingly broken, so need to test these later (most notably the projectiles)
            new AttackInfo { attackKey="SeaDragonBite", defaultDamage=300f, configValue=() => Plugin.config.SeaDragonBite, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonSwat", defaultDamage=70f, configValue=() => Plugin.config.SeaDragonSwat, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonShove", defaultDamage=250f, configValue=() => Plugin.config.SeaDragonShove, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonBurningChunk", defaultDamage=10f, configValue=() => Plugin.config.SeaDragonBurningChunk, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonLavaMeteor", defaultDamage=40f, configValue=() => Plugin.config.SeaDragonLavaMeteor, isGenericAttack=false }
        })},
        { (TechType.SeaTreader, new AttackInfo[] {
            new AttackInfo { attackKey="SeaTreaderPeck", defaultDamage=40f, configValue=() => Plugin.config.SeaTreaderPeck, isGenericAttack=false }
        })},
        { (TechType.Stalker, new AttackInfo[] {
            new AttackInfo { attackKey="StalkerBite", defaultDamage=30f, configValue=() => Plugin.config.StalkerBite }
        })},
        { (TechType.SpikePlant, new AttackInfo[] { //Tiger Plant
            new AttackInfo { attackKey="TigerPlantThorn", defaultDamage=10f, configValue=() => Plugin.config.TigerPlantThorn, isGenericAttack=false }
        })},
        { (TechType.Warper, new AttackInfo[] {
            new AttackInfo { attackKey="WarperClaw", defaultDamage=30f, configValue=() => Plugin.config.WarperClaw, isGenericAttack=false },
            new AttackInfo { attackKey="WarperWarpBall", defaultDamage=10f, configValue=() => Plugin.config.WarperWarpBall, isGenericAttack=false }
        })},
    };
}

internal class AttackInfo
{
    internal required string attackKey; // The string used to identify the specific attack among possibly several (also, if the attack isn't generic, the key will be used to determine its implementation)
    internal required float defaultDamage; // The damage the attack deals by default (will be used to calculate damage changes)
    internal required Func<float> configValue; // Stores a function to retrieve the user-selected config value associated with the attack (REMEMBER!!, '() =>', or arrow function, is just a shorthand function)
    internal bool isGenericAttack = true; // Whether the attack is simply a MeleeAttack component, or requires more specifics to apply changes (true by default)
}