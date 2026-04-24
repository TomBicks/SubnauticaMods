using System.Collections.Generic;

namespace CreatureConfigDamage;

internal class DamageDefaults
{
    public static readonly Dictionary<string, float> defaults = new Dictionary<string, float>()
    {
        { "AmpeelBiteDmg",30F },
        { "AmpeelShockDmg",15F },
        { "AmpeelCyclopsDmg",50F },
        { "BiterDmg",7F },
        { "BleederDmg",5F },
        { "BlighterDmg",7F },
        { "BloodCrawlerDmg",5F },
        { "BonesharkDmg",30F },
        { "CaveCrawlerDmg",5F },
        { "CrabsnakeDmg",35F },
        { "CrabsquidDmg",40F },
        { "CrashfishDmg",50F },
        { "DroopingStingerDmg", 50F },
        { "GasopodGasPodDmg",10F },
        { "GhostLeviathanDmg",85F },
        { "GhostLeviathanCyclopsDmg",250F },
        { "GhostLeviathanJuvenileDmg",55F },
        { "GhostLeviathanJuvenileCyclopsDmg",220F },
        { "LavaLizardBiteDmg",30F },
        { "LavaLizardLavaRockDmg",15F },
        { "MesmerDmg",35F },
        { "ReaperLeviathanDmg",80F },
        { "ReaperLeviathanCyclopsDmg",220F },
        { "RiverProwlerDmg",30F },
        { "SandsharkDmg",30F },
        { "SeaDragonLeviathanBiteDmg",300F },
        { "SeaDragonLeviathanSwatDmg",70F },
        { "SeaDragonLeviathanShoveDmg",250F },
        { "SeaDragonLeviathanBurningChunkDmg",10F },
        { "SeaDragonLeviathanLavaMeteorDmg",40F },
        { "SeaTreaderLeviathanDmg",40F },
        { "StalkerDmg", 30F },
        { "TigerPlantDmg", 10F },
        { "WarperClawDmg", 30F },
        { "WarperWarpDmg", 10F }
    };

    //MUCH CLEANER!!
    public static readonly List<(TechType, AttackInfo[])> attacks = new List<(TechType, AttackInfo[])>()
    {
        { (TechType.Biter, new AttackInfo[] {
            new AttackInfo { attackKey="BiterBite", defaultDamage=7f }
        })},
        { (TechType.Stalker, new AttackInfo[] {
            new AttackInfo { attackKey="StalkerBite", defaultDamage=30f }
        })},
        { (TechType.Shocker, new AttackInfo[] {
            new AttackInfo { attackKey="AmpeelBiteDmg", defaultDamage=30f },
            new AttackInfo { attackKey="AmpeelShockDmg", defaultDamage=15f, isGenericAttack=false } //NOW, IT'LL AUTOMATICALLY USE GENERIC OR NOT! (LIKELY WILL BE A SWITCH STATEMENT ON THE OTHER SIDE)
        })},
    };
}

internal class AttackInfo
{
    internal string attackKey; // The string used to identify the specific attack (one creature may have several different attacks)
    internal float defaultDamage; // The damage the attack deals by default (will be used to calculate damage changes)
    internal bool isGenericAttack = true; // Whether the attack is simply a MeleeAttack component, or requires more specifics to apply changes (true by default)
}