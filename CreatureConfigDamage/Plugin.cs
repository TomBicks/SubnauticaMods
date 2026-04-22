using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using CreatureConfigDamage.Items.Equipment;
using Nautilus.Options.Attributes;
using System.Collections.Generic;
using Nautilus.Handlers;

namespace CreatureConfigDamage;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    internal static DamageConfig config { get; } = OptionsPanelHandler.RegisterModOptions<DamageConfig>();

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        Logger = base.Logger;
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}