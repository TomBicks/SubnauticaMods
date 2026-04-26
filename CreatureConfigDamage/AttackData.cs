using System.Collections.Generic;

namespace CreatureConfigDamage;

internal class AttackData
{
    public static readonly List<(TechType, AttackInfo[])> creatureAttacks = new List<(TechType, AttackInfo[])>()
    {
        { (TechType.Shocker, new AttackInfo[] { //Ampeel
            new AttackInfo { attackKey="AmpeelBite", defaultDamage=30f },
            new AttackInfo { attackKey="AmpeelShock", defaultDamage=15f, isGenericAttack=false },
            new AttackInfo { attackKey="AmpeelCyclopsShock", defaultDamage=50f, isGenericAttack=false }
        })},
        { (TechType.Biter, new AttackInfo[] {
            new AttackInfo { attackKey="BiterBite", defaultDamage=7f }
        })},
        { (TechType.Bleeder, new AttackInfo[] {
            new AttackInfo { attackKey="BleederSuck", defaultDamage=5f, isGenericAttack=false }
        })},
        { (TechType.Blighter, new AttackInfo[] {
            new AttackInfo { attackKey="BlighterBite", defaultDamage=7f }
        })},
        { (TechType.Shuttlebug, new AttackInfo[] { //Blood Crawler
            new AttackInfo { attackKey="BloodCrawlerBite", defaultDamage=5f }
        })},
        { (TechType.BoneShark, new AttackInfo[] {
            new AttackInfo { attackKey="BoneSharkBite", defaultDamage=30f }
        })},
        { (TechType.CaveCrawler, new AttackInfo[] {
            new AttackInfo { attackKey="CaveCrawlerBite", defaultDamage=5f }
        })},
        { (TechType.Crabsnake, new AttackInfo[] {
            new AttackInfo { attackKey="CrabsnakeBite", defaultDamage=35f, isGenericAttack=false }
        })},
        { (TechType.CrabSquid, new AttackInfo[] {
            new AttackInfo { attackKey="CrabSquidBite", defaultDamage=40f }
        })},
        { (TechType.Crash, new AttackInfo[] { //Crashfish
            new AttackInfo { attackKey="CrashfishExplosion", defaultDamage=50f, isGenericAttack=false }
        })},
        { (TechType.GasPod, new AttackInfo[] { //Gasopod's Gas Pods (the damage is tied to the pods, not the creature itself)
            new AttackInfo { attackKey="GasPodPoison", defaultDamage=10f, isGenericAttack=false }
        })},
        { (TechType.GhostLeviathan, new AttackInfo[] {
            new AttackInfo { attackKey="GhostBite", defaultDamage=85f, isGenericAttack=false },
            new AttackInfo { attackKey="GhostCyclopsBite", defaultDamage=250f, isGenericAttack=false }
        })},
        { (TechType.GhostLeviathanJuvenile, new AttackInfo[] {
            new AttackInfo { attackKey="GhostJuvBite", defaultDamage=55f, isGenericAttack=false },
            new AttackInfo { attackKey="GhostJuvCyclopsBite", defaultDamage=220f, isGenericAttack=false }
        })},
        { (TechType.LavaLizard, new AttackInfo[] {
            new AttackInfo { attackKey="LavaLizardBite", defaultDamage=30f, },
            new AttackInfo { attackKey="LavaLizardLavaRock", defaultDamage=15f, isGenericAttack=false }
        })},
        { (TechType.Mesmer, new AttackInfo[] {
            new AttackInfo { attackKey="MesmerBite", defaultDamage=35f }
        })},
        { (TechType.ReaperLeviathan, new AttackInfo[] {
            new AttackInfo { attackKey="ReaperBite", defaultDamage=80f, isGenericAttack=false },
            new AttackInfo { attackKey="ReaperCyclopsBite", defaultDamage=220f, isGenericAttack=false }
        })},
        { (TechType.SpineEel, new AttackInfo[] { //River Prowler
            new AttackInfo { attackKey="StalkerBite", defaultDamage=30f }
        })},
        { (TechType.Sandshark, new AttackInfo[] {
            new AttackInfo { attackKey="SandsharkBite", defaultDamage=30f }
        })},
        { (TechType.SeaDragon, new AttackInfo[] { //TODO!! SeaDragon was accordingly broken, so need to test these later (most notably the projectiles)
            new AttackInfo { attackKey="SeaDragonBite", defaultDamage=300f, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonSwat", defaultDamage=70f, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonShove", defaultDamage=250f, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonBurningChunk", defaultDamage=10f, isGenericAttack=false },
            new AttackInfo { attackKey="SeaDragonLavaMeteor", defaultDamage=40f, isGenericAttack=false }
        })},
        { (TechType.SeaTreader, new AttackInfo[] {
            new AttackInfo { attackKey="SeaTreaderPeck", defaultDamage=40f, isGenericAttack=false }
        })},
        { (TechType.SpikePlant, new AttackInfo[] { //Tiger Plant
            new AttackInfo { attackKey="TigerPlantThorn", defaultDamage=10f, isGenericAttack=false }
        })},
        { (TechType.Stalker, new AttackInfo[] {
            new AttackInfo { attackKey="StalkerBite", defaultDamage=30f }
        })},
        { (TechType.Warper, new AttackInfo[] {
            new AttackInfo { attackKey="WarperClaw", defaultDamage=30f, isGenericAttack=false },
            new AttackInfo { attackKey="WarperWarpBall", defaultDamage=10f, isGenericAttack=false }
        })},
    };
}

internal class AttackInfo
{
    internal string attackKey; // The string used to identify the specific attack among possibly several (also, if the attack isn't generic, the key will be used to determine its implementation)
    internal float defaultDamage; // The damage the attack deals by default (will be used to calculate damage changes)
    internal bool isGenericAttack = true; // Whether the attack is simply a MeleeAttack component, or requires more specifics to apply changes (true by default)
}