using PCBSModloader;
using UnityEngine;

namespace Texture_And_Material_Replacer
{
    class ModloaderMod : Mod
    {
        public override string ID { get { return "Texture And Material Replacer"; } }

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

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ConfigHolder.Instance.ReloadConfigurations();
                State.Instance.updateMessage = true;
            }
        }

        public override void OnGUI()
        {
            if (State.Instance.updateMessage)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.cyan;
                style.fontSize = 30;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(Screen.width / 2 - 100, 10f, 200f, 30f), "Replacement configurations reloaded, please reload your save game!", style);
            }

        }
    }
}
