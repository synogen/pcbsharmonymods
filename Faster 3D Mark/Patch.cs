using Harmony;
using System.IO;

namespace Faster_3D_Mark
{
    [HarmonyPatch(typeof(SaveLoadSystem))]
    [HarmonyPatch("LoadGame")]
    class Patch
    {
        static bool Prefix()
        {
            var seconds = 9f; // default value
            if (File.Exists(ModloaderMod.Instance.Modpath + "/seconds_to_run.txt"))
            {
                var lines = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/seconds_to_run.txt");
                if (lines.Length > 0)
                {
                    float.TryParse(lines[lines.Length - 1], out seconds);
                }
            }
            ProgramConstants.s_max3DMarkTestDuration = seconds / 3;
            PCBSModloader.ModLogs.Log("Changed ProgramConstants.s_max3DMarkTestDuration to " + ProgramConstants.s_max3DMarkTestDuration);
            return true;
        }
    }
}
