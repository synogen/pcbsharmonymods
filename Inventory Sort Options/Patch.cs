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
        static void Prefix(PartDesc.ShopCategory category, ref List<PartInstance> ___itemsDisplayedInInventory)
        {
            SortBy sort = SortOptions.Instance.forCategory(category);

            switch (sort)
            {
                case SortBy.Default:
                    // keep order as it is
                    break;
                case SortBy.NewestFirst:
                    ___itemsDisplayedInInventory.Reverse();
                    break;
                case SortBy.PriceAscending:
                    ___itemsDisplayedInInventory.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetPart().m_price < b.GetPart().m_price ? -1 : a.GetPart().m_price > b.GetPart().m_price ? 1 : 0;
                        }
                    );
                    break;
                case SortBy.PriceDescending:
                    ___itemsDisplayedInInventory.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetPart().m_price < b.GetPart().m_price ? 1 : a.GetPart().m_price > b.GetPart().m_price ? -1 : 0;
                        }
                    );
                    break;
                case SortBy.NameAscending:
                    ___itemsDisplayedInInventory.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetPart().m_uiShopName.CompareTo(b.GetPart().m_uiShopName);
                        }
                    );
                    break;
                case SortBy.NameDescending:
                    ___itemsDisplayedInInventory.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetPart().m_uiShopName.CompareTo(b.GetPart().m_uiShopName) * -1;
                        }
                    );
                    break;
                default:
                    break;
            }
            
            
        }
    }
}
