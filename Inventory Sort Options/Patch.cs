using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory_Sort_Options
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("ConstructInventory")]
    class PatchInit
    {
        static void Prefix(Inventory __instance, PartDesc.ShopCategory ___m_currentCategory)
        {
            if (File.Exists(ModloaderMod.Instance.Modpath + "/ui.assetbundle"))
            {
                AssetBundle uiBundle = AssetBundle.LoadFromFile(ModloaderMod.Instance.Modpath + "/ui.assetbundle");

                Dropdown dropdown = UnityEngine.Object.Instantiate(uiBundle.LoadAsset<Dropdown>("Dropdown"), __instance.parent, false);
                dropdown.onValueChanged = new Dropdown.DropdownEvent();
                dropdown.onValueChanged.AddListener(delegate(int choice)
                    {
                        switch (choice)
                        {
                            case 0:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.Default);
                                break;
                            case 1:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.NewestFirst);
                                break;
                            case 2:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.PriceAscending);
                                break;
                            case 3:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.PriceDescending);
                                break;
                            case 4:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.NameAscending);
                                break;
                            case 5:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.NameDescending);
                                break;
                            default:
                                SortOptions.Instance.setSortFor(___m_currentCategory, SortBy.Default);
                                break;
                        }
                        __instance.UpdateInventory(___m_currentCategory);
                    }
                );
            }
        }
    }


    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("UpdateInventory")]
    class PatchUpdate
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
