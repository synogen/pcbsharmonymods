using PCBSModloader;

namespace Portable_Run_And_No_Restart_Install
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Portable Run And No Restart Install"; } }

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
