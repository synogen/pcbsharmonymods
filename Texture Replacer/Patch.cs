using Harmony;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Texture_And_Material_Replacer
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

        public Dictionary<string, Color32> materialColors = new Dictionary<string, Color32>();

        public Dictionary<string, Material> materialReplacements = new Dictionary<string, Material>();

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

            string[] materialConfigurations = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Material Colors.conf");
            foreach (string materialConfiguration in materialConfigurations)
            {
                string[] materialConfig = materialConfiguration.Split('|');
                if (materialConfig.Length == 2)
                {
                    string[] colorBytes = materialConfig[1].Split(',');
                    materialColors.Add(materialConfig[0], new Color32(
                        byte.Parse(colorBytes[0]),
                        byte.Parse(colorBytes[1]),
                        byte.Parse(colorBytes[2]),
                        byte.Parse(colorBytes[3])
                        ));
                    File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", "Added color configuration for " + materialConfig[0] + " => " + materialConfig[1] + "\n");
                }
            }

            if (File.Exists(ModloaderMod.Instance.Modpath + "/materials.assetbundle"))
            {
                AssetBundle materialBundle = AssetBundle.LoadFromFile(ModloaderMod.Instance.Modpath + "/materials.assetbundle");

                foreach (Material material in materialBundle.LoadAllAssets<Material>())
                {
                    materialReplacements.Add(material.name, material);
                    File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", "Added replacement configuration for " + material.name + " => " + material + "\n");
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
            Renderer[] renderers = __result.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer.material.mainTexture != null && typeof(Texture2D).IsInstanceOfType(renderer.material.mainTexture))
                {
                    if (ConfigHolder.Instance.textureReplacements.ContainsKey(renderer.material.mainTexture.name))
                    {
                        ConfigHolder.Instance.textureReplacements.GetValueSafe(renderer.material.mainTexture.name).LoadImageIntoTexture(renderer.material.mainTexture as Texture2D);
                    }
                }
                foreach (Material material in renderer.materials)
                {
                    string materialName = material.name.Replace("(Instance)", "").Trim();
                    if (ConfigHolder.Instance.materialColors.ContainsKey(materialName))
                    {
                        material.color = ConfigHolder.Instance.materialColors.GetValueSafe(materialName);
                    }
                    if (ConfigHolder.Instance.materialReplacements.ContainsKey(materialName))
                    {
                        material.CopyPropertiesFromMaterial(ConfigHolder.Instance.materialReplacements.GetValueSafe(materialName));
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

    [HarmonyPatch(typeof(SaveLoadSystem))]
    [HarmonyPatch("LoadSaveGame")]
    class PatchLevelLoad
    {
        
        static void Postfix()
        {
            foreach (Renderer o in UnityEngine.Object.FindObjectsOfType<Renderer>())
            {
                foreach (Material material in o.materials)
                {
                    if (material.mainTexture != null)
                    {
                        if (ConfigHolder.Instance.imageReplacements.ContainsKey(material.mainTexture.name))
                        {
                            ConfigHolder.Instance.imageReplacements.GetValueSafe(material.mainTexture.name).LoadImageIntoTexture(material.mainTexture as Texture2D);
                        }

                    }
                    string materialName = material.name.Replace("(Instance)", "").Trim();
                    if (ConfigHolder.Instance.materialColors.ContainsKey(materialName))
                    {
                        material.color = ConfigHolder.Instance.materialColors.GetValueSafe(materialName);
                    }
                    if (ConfigHolder.Instance.materialReplacements.ContainsKey(materialName))
                    {
                        material.CopyPropertiesFromMaterial(ConfigHolder.Instance.materialReplacements.GetValueSafe(materialName));
                    }

                }
            }
        }
    }
}
