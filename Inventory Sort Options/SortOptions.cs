using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inventory_Sort_Options
{
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

        // TODO dictionary with current sort options for every category?

        private SortOptions()
        {
            // TODO load dropdown from assetbundle and link value changed function
        }
    }
}
