using Harmony;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Texture_Replacer
{
    class ConfigHolder
    {
        private static ConfigHolder configHolder;

        public static ConfigHolder Instance
        {
            get
            {
                if (configHolder == null)
                {
                    configHolder = new ConfigHolder();
                }
                return configHolder;
            }
        }

        public Dictionary<string, WWW> textureReplacements = new Dictionary<string, WWW>();

        public Dictionary<string, WWW> imageReplacements = new Dictionary<string, WWW>();

        private ConfigHolder()
        {
            File.Delete(ModloaderMod.Instance.Modpath + "/replacer.log");

            string[] textureConfigurations = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Parts Texture Replacer.conf");
            foreach (string textureConfigLine in textureConfigurations)
            {
                string[] textureConfig = textureConfigLine.Split('|');
                if (textureConfig.Length == 2)
                {
                    textureReplacements.Add(textureConfig[0], new WWW(("file:///" + ModloaderMod.Instance.Modpath + "/" + textureConfig[1]).Replace(" ", "%20").Replace("\\", "/")));
                    File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", "Added replacement configuration for " + textureConfig[0] + " => " + textureConfig[1] + "\n");
                }
            }

            string[] imageConfigurations = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Image Replacer.conf");
            foreach (string imageConfigLine in imageConfigurations)
            {
                string[] imageConfig = imageConfigLine.Split('|');
                if (imageConfig.Length == 2)
                {
                    imageReplacements.Add(imageConfig[0], new WWW(("file:///" + ModloaderMod.Instance.Modpath + "/" + imageConfig[1]).Replace(" ", "%20").Replace("\\", "/")));
                    File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", "Added replacement configuration for " + imageConfig[0] + " => " + imageConfig[1] + "\n");
                }
            }
        }
    }

    [HarmonyPatch(typeof(PartsDatabase))]
    [HarmonyPatch("InstantiatePart")]
    class PatchPartTextures
    {
        static GameObject Postfix(GameObject __result)
        {
            MeshRenderer[] meshRenderers = __result.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer.material.mainTexture != null && typeof(Texture2D).IsInstanceOfType(meshRenderer.material.mainTexture))
                {
                    if (ConfigHolder.Instance.textureReplacements.ContainsKey(meshRenderer.material.mainTexture.name))
                    {
                        ConfigHolder.Instance.textureReplacements.GetValueSafe(meshRenderer.material.mainTexture.name).LoadImageIntoTexture(meshRenderer.material.mainTexture as Texture2D);
                    }
                }
            }
            return __result;
        }
    }

    [HarmonyPatch(typeof(Image))]
    [HarmonyPatch("sprite", PropertyMethod.Getter)]
    class PatchOthers
    {
        static void Postfix(ref Sprite __result)
        {
            if (__result != null && __result.texture != null)
            {
                if (ConfigHolder.Instance.imageReplacements.ContainsKey(__result.texture.name))
                {
                    ConfigHolder.Instance.imageReplacements.GetValueSafe(__result.texture.name).LoadImageIntoTexture(__result.texture as Texture2D);
                }
            }
        }
    }

    [HarmonyPatch(typeof(LevelLoadPersistency))]
    [HarmonyPatch("OnSceneLoaded")]
    class PatchLevelLoad
    {
        static void Postfix(Scene scene)
        {
            foreach (Renderer o in UnityEngine.Object.FindObjectsOfType<Renderer>())
            {
                foreach (Material m in o.materials)
                {
                    if (m.mainTexture != null)
                    {
                        if (ConfigHolder.Instance.imageReplacements.ContainsKey(m.mainTexture.name))
                        {
                            ConfigHolder.Instance.imageReplacements.GetValueSafe(m.mainTexture.name).LoadImageIntoTexture(m.mainTexture as Texture2D);
                        }

                    }
                }   
            }
        }
    }
}
