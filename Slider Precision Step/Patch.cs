using Harmony;
using UnityEngine.UI;

namespace Slider_Precision_Step
{
    [HarmonyPatch(typeof(Slider))]
    [HarmonyPatch("stepSize", PropertyMethod.Getter)]
    class PatchLevelLoad
    {
        static float Postfix(float __result)
        {
            return 1;
        }
    }
}
