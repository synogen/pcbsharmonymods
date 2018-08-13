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

        private Dictionary<PartDesc.ShopCategory, SortBy> currentSort = new Dictionary<PartDesc.ShopCategory, SortBy>();

        private SortOptions()
        {
            // TODO load sort settings from file?
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

        public void setSortFor(PartDesc.ShopCategory category, SortBy sortBy)
        {
            if (!currentSort.ContainsKey(category))
            {
                currentSort.Add(category, sortBy);
            }
            else
            {
                currentSort[category] = sortBy;
            }
        }
    }
}
