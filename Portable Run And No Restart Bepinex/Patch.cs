using HarmonyLib;
using System;

namespace Portable_Run_And_No_Restart_Bepinex
{
    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("Start")]
    class AddProgramAppStartPatch
    {
        static bool Prefix(AddProgramApp __instance)
        {
            AddProgramAppLogic.InstanceFor(__instance).Init();
            return false;
        }

        static void Postfix(AddProgramApp __instance)
        {
            AddProgramAppLogic.InstanceFor(__instance).SetPortableMode();
        }
    }

    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("OnDestroy")]
    class AddProgramAppOnDestroyPatch
    {
        static void Postfix(AddProgramApp __instance)
        {
            AddProgramAppLogic.Cleanup(__instance);
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

    [HarmonyPatch(typeof(OS))]
    [HarmonyPatch("OnDestroy")]
    class OSOnDestroyPatch
    {
        static void Postfix(OS __instance)
        {
            OSLogic.Cleanup(__instance);
        }
    }
}
