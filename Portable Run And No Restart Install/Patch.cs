using Harmony;

namespace Portable_Run_And_No_Restart_Install
{
    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("Start")]
    class AddProgramAppStartPatch
    {
        static bool Prefix()
        {
            return true;
        }

        static void Postfix(AddProgramApp __instance)
        {
            new AddProgramAppLogic(__instance);
        }
    }

    [HarmonyPatch(typeof(OS))]
    [HarmonyPatch("Awake")]
    class OSAwakePatch
    {
        static bool Prefix()
        {
            return true;
        }

        static void Postfix(OS __instance)
        {
            new OSLogic(__instance);
        }
    }
}
