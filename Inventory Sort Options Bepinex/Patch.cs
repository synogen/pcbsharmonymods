using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
            string assetBundlePath = Path.Combine(Plugin.PluginDirectory, "ui.assetbundle");

            if (File.Exists(assetBundlePath))
            {
                try
                {
                    if (dropdownPrefab == null || sortDropdown == null)
                    {
                        if (dropdownPrefab == null)
                        {
                            AssetBundle uiBundle = AssetBundle.LoadFromFile(assetBundlePath);
                            dropdownPrefab = uiBundle.LoadAsset<GameObject>("Dropdown");
                        }

                        RectTransform itemListRect = __instance.m_itemList.GetComponent<RectTransform>();
                        Transform dropdownParent = itemListRect.parent;

                        GameObject goDropdown = UnityEngine.Object.Instantiate(dropdownPrefab, dropdownParent, false);
                        RectTransform dropRect = goDropdown.GetComponent<RectTransform>();
                        dropRect.anchorMin = new Vector2(1f, 1f);
                        dropRect.anchorMax = new Vector2(1f, 1f);
                        dropRect.pivot = new Vector2(1f, 1f);
                        dropRect.anchoredPosition = new Vector2(-3f, -48f);
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
                                __instance.ConstructInventory(false);
                            }
                        );
                    }
                    PartDesc.ShopCategory category = ReflectionUtils.Get<PartDesc.ShopCategory>("m_currentCategory", __instance);
                    sortDropdown.value = (int)SortOptions.Instance.forCategory(category);
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e.Message);
                    throw;
                }

            }
        }
    }


    [HarmonyPatch(typeof(Inventory))]
    [HarmonyPatch("UpdateInventory")]
    class PatchUpdate
    {
        static void Prefix(PartDesc.ShopCategory type, ref List<PartInstance> ___m_itemsPendingCreation)
        {
            SortBy sort = SortOptions.Instance.forCategory(type);

            switch (sort)
            {
                case SortBy.Default:
                    // keep order as it is
                    break;
                case SortBy.NewestFirst:
                    ___m_itemsPendingCreation.Reverse();
                    break;
                case SortBy.PriceAscending:
                    ___m_itemsPendingCreation.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetResaleValue().CompareTo(b.GetResaleValue());
                        }
                    );
                    break;
                case SortBy.PriceDescending:
                    ___m_itemsPendingCreation.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return b.GetResaleValue().CompareTo(a.GetResaleValue());
                        }
                    );
                    break;
                case SortBy.NameAscending:
                    ___m_itemsPendingCreation.Sort(
                        delegate (PartInstance a, PartInstance b)
                        {
                            return a.GetPart().m_uiShopName.CompareTo(b.GetPart().m_uiShopName);
                        }
                    );
                    break;
                case SortBy.NameDescending:
                    ___m_itemsPendingCreation.Sort(
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
