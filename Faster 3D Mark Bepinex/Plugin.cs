using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace Faster_3D_Mark
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_NAME);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo("Harmony patches applied.");
        }
    }
}
