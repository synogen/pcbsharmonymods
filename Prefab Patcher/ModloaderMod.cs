using PCBSModloader;

namespace Prefab_Patcher
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Prefab Patcher"; } }

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
