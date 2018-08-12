using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inventory_Sort_Options
{
    public enum SortBy
    {
        Default,
        NewestFirst,
        PriceAscending,
        PriceDescending,
        NameAscending,
        NameDescending
    }

    class SortOptions
    {
        private static SortOptions singletonInstance;

        public static SortOptions Instance
        {
            get
            {
                singletonInstance = singletonInstance != null ? singletonInstance : new SortOptions();
                return singletonInstance;
            }
        }

        private Dictionary<PartDesc.ShopCategory, SortBy> currentSort;

        private SortOptions()
        {
            // TODO load dropdown from assetbundle and link value changed function
        }

        public SortBy forCategory(PartDesc.ShopCategory category)
        {
            if (currentSort.ContainsKey(category))
            {
                return currentSort.GetValueSafe(category);
            }
            else
            {
                return SortBy.Default;
            }
        }
    }
}
