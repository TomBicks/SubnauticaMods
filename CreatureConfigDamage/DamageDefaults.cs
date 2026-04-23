using System.Collections.Generic;

namespace CreatureConfigDamage;

internal class DamageDefaults
{
    public static readonly Dictionary<string, float> defaultDamageValues = new Dictionary<string, float>()
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
}
