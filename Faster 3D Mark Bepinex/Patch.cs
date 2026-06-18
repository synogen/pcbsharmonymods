using HarmonyLib;
using System.IO;
using System.Reflection;

namespace Faster_3D_Mark
{
    [HarmonyPatch(typeof(PartsDatabase))]
    [HarmonyPatch("Load")]
    class Patch
    {
        static void Postfix()
        {
            var seconds = 9f;
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = Path.Combine(modPath, "seconds_to_run.txt");

            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                if (lines.Length > 0)
                {
                    float.TryParse(lines[lines.Length - 1], out seconds);
                }
            }

            ProgramConstants.s_max3DMarkTestDuration = seconds / 3;
            Plugin.Logger.LogInfo("Changed ProgramConstants.s_max3DMarkTestDuration (3D Mark per stage duration) to " + ProgramConstants.s_max3DMarkTestDuration);
        }
    }
}
