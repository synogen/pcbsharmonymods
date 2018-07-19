using Harmony;

namespace Order_Templates
{
    [HarmonyPatch(typeof(CheckoutFinalRow))]
    [HarmonyPatch("Set")]
    class Patch
    {
        static void Postfix(CheckoutFinalRow __instance)
        {
            new CheckoutFinalRowLogic(__instance);
        }
    }
}
