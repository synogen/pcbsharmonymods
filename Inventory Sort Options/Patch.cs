using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Inventory_Sort_Options
{
    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("ConstructInventory")]
    class PatchInit
    {
        static GameObject dropdownPrefab;

        static Dropdown sortDropdown;

        static void Prefix(Inventory __instance)
        {
            if (File.Exists(ModloaderMod.Instance.Modpath + "/ui.assetbundle"))
            {
                try
                {
                    
                    if (dropdownPrefab == null)
                    {
                        AssetBundle uiBundle = AssetBundle.LoadFromFile(ModloaderMod.Instance.Modpath + "/ui.assetbundle");
                        dropdownPrefab = uiBundle.LoadAsset<GameObject>("Dropdown");

                        GameObject goDropdown = UnityEngine.Object.Instantiate(dropdownPrefab, __instance.parent.parent.parent, false);
                        goDropdown.transform.localPosition = new Vector3(goDropdown.transform.localPosition.x + 175, goDropdown.transform.localPosition.y + 280);
                        sortDropdown = goDropdown.GetComponent<Dropdown>();
                        sortDropdown.onValueChanged = new Dropdown.DropdownEvent();
                        sortDropdown.onValueChanged.AddListener(delegate (int choice)
                            {
                                PartDesc.ShopCategory currentCategory = ReflectionUtils.Get<PartDesc.ShopCategory>("m_currentCategory", __instance);
                                switch (choice)
                                {
                                    case 0:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.Default);
                                        break;
                                    case 1:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.NewestFirst);
                                        break;
                                    case 2:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.PriceAscending);
                                        break;
                                    case 3:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.PriceDescending);
                                        break;
                                    case 4:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.NameAscending);
                                        break;
                                    case 5:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.NameDescending);
                                        break;
                                    default:
                                        SortOptions.Instance.setSortFor(currentCategory, SortBy.Default);
                                        break;
                                }
                                __instance.ConstructInventory();
                            }
                        );
                    }
                    PartDesc.ShopCategory category = ReflectionUtils.Get<PartDesc.ShopCategory>("m_currentCategory", __instance);
                    sortDropdown.value = (int)SortOptions.Instance.forCategory(category);
                }
                catch (Exception e)
                {
                    PCBSModloader.ModLogs.Log(e.Message);
                    throw;
                }
                
            }
        }
    }


    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("UpdateInventory")]
    class PatchUpdate
    {
        static void Prefix(PartDesc.ShopCategory type, ref List<PartInstance> ___itemsDisplayedInInventory)
        {
            SortBy sort = SortOptions.Instance.forCategory(type);

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
