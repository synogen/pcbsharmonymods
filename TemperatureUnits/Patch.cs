using Harmony;

namespace TemperatureUnits
{
    [HarmonyPatch(typeof(StringExt))]
    [HarmonyPatch("ToCelsius")]
    class Patch
    {
        static bool Prefix()
        {
            return false;
        }

        static string Postfix(string __result, float value)
        {
            return (value * 9/5 + 32).ToString("N2") + "°F";
        }
    }
}
