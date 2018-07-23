using PCBSModloader;

namespace Show_Inventory_Stock_In_Shop
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Show Inventory Stock In Shop"; } }

        public override string Version { get { return "1"; } }

        public override string Author { get { return "synogen"; } }

        private static ModloaderMod singletonInstance;

        public static ModloaderMod Instance {
            get
            {
                singletonInstance = singletonInstance != null ? singletonInstance : new ModloaderMod();
                return singletonInstance;
            }
        }

    }
}
