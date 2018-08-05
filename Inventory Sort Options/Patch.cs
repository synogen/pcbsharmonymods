using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inventory_Sort_Options
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("UpdateInventory")]
    class Patch
    {
        static void Prefix(ref List<PartInstance> ___itemsDisplayedInInventory)
        {
            // TODO sort logic depending on SortOptions?
            ___itemsDisplayedInInventory.Reverse();
        }
    }
}
