using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace Show_Detailed_Inventory_In_Stock
{
    [HarmonyPatch(typeof(ShopItem))]
    [HarmonyPatch("UpdateInventoryCountText")]
    class ShopItemUpdateInventoryCountTextPatch
    {
        static void Postfix(ShopItem __instance)
        {
            if (__instance.m_inventoryCount == null || !__instance.m_inventoryCount.gameObject.activeSelf)
                return;

            __instance.m_inventoryCount.supportRichText = true;

            string partId = __instance.Entry.m_part.m_id;
            IEnumerable<PartInstance> inventory = CareerStatus.Get().GetInventory();
            int total = __instance.Entry.m_inventory;

            int newCount = inventory.Count(p => p.GetPartId() == partId && !p.IsBroken() && p.IsNew());
            int used = inventory.Count(p => p.GetPartId() == partId && !p.IsBroken() && !p.IsNew());
            int broken = inventory.Count(p => p.GetPartId() == partId && p.IsBroken());

            string t = "<size=14><b>\u2610</b></size>";
            string text = t + " " + total + "  (";
            text += "<color=#4EC959><size=16><b>\u2605</b></size>" + newCount + "</color> / ";
            text += "<color=#F5C542><size=16><b>\u2605</b></size>" + used + "</color> / ";
            text += "<color=#E84430><size=14><b>\u271D</b></size>" + broken + "</color>)";
            __instance.m_inventoryCount.text = text;
        }
    }
}
