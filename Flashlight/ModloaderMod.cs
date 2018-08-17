using PCBSModloader;
using UnityEngine;

namespace Flashlight
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Flashlight"; } }

        public override string Version { get { return "1"; } }

        public override string Author { get { return "synogen"; } }

        private static ModloaderMod singletonInstance;

        public static ModloaderMod Instance
        {
            get
            {
                singletonInstance = singletonInstance != null ? singletonInstance : new ModloaderMod();
                return singletonInstance;
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && Flashlight.Instance.currentLight != null)
            {
                Flashlight.Instance.currentLight.enabled = !Flashlight.Instance.currentLight.enabled;
            }
        }
    }
}
