using Harmony;
using UnityEngine;

namespace Flashlight
{
    [HarmonyPatch(typeof(GameController))]
    [HarmonyPatch("SetCamera")]
    class PatchLevelLoad
    {
        static void Postfix(ref Camera ___m_activeCamera)
        {
            Light light = ___m_activeCamera.gameObject.GetOrAddComponent<Light>();
            Flashlight.Instance.SetCurrentFlashlight(ref light);
        }
    }
}
