using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System;
using System.Linq;

namespace Portable_Run_And_No_Restart_Bepinex;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_NAME);
        PatchAllInNamespace(harmony, "Portable_Run_And_No_Restart_Bepinex");
    }

    private void PatchAllInNamespace(Harmony harmony, string targetNamespace)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes().Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith(targetNamespace)).ToList();

        foreach (var type in types)
        {
            if (type.GetCustomAttributes(typeof(HarmonyPatch), true).Any() ||
                type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Any(m => m.GetCustomAttributes(typeof(HarmonyPatch), true).Any()))
            {
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                    Logger.LogInfo($"Patched {type.FullName}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to patch {type.FullName}: {ex}");
                }
            }
        }
    }
}
