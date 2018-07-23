using Harmony;
using System;

namespace Portable_Run_And_No_Restart_Install
{
    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("Start")]
    class AddProgramAppStartPatch
    {
        static void Prefix(AddProgramApp __instance)
        {
            AddProgramAppLogic.InstanceFor(__instance).Init();
        }

        static void Postfix(AddProgramApp __instance)
        {
            AddProgramAppLogic.InstanceFor(__instance).SetPortableMode();
        }
    }

    [HarmonyPatch(typeof(OS))]
    [HarmonyPatch("Awake")]
    class OSAwakePatch
    {
        static void Postfix(OS __instance)
        {
            OSLogic.InstanceFor(__instance).Init();
        }
    }
}
