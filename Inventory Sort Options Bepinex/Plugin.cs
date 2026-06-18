using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;

namespace Inventory_Sort_Options
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        internal static string PluginDirectory { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;
            PluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"All patches applied for {MyPluginInfo.PLUGIN_GUID}.");
        }
    }
}
