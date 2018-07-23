﻿using PCBSModloader;

namespace Texture_Replacer
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Texture Replacer"; } }

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