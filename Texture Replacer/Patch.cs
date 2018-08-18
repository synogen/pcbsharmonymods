using Harmony;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Texture_And_Material_Replacer
{
    abstract class ReplacementConfiguration
    {
        public string PartID { get; set; }
    }

    class TextureConfiguration : ReplacementConfiguration
    {
        public WWW TexturePath { get; set; }

        public TextureConfiguration(WWW texturePath)
        {
            PartID = null;
            TexturePath = texturePath;
        }

        public TextureConfiguration(string partID, WWW texturePath)
        {
            PartID = partID;
            TexturePath = texturePath;
        }
    }

    class MaterialConfiguration : ReplacementConfiguration
    {
        public Color32 Color { get; set; }
        public Material Material { get; set; }

        public MaterialConfiguration(Color32 color)
        {
            PartID = null;
            Color = color;
        }

        public MaterialConfiguration(string partID, Color32 color)
        {
            PartID = partID;
            Color = color;
        }

        public MaterialConfiguration(Material material)
        {
            PartID = null;
            Material = material;
        }

        public MaterialConfiguration(string partID, Material material)
        {
            PartID = partID;
            Material = material;
        }
    }

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

        public Dictionary<string, List<TextureConfiguration>> textureReplacements = new Dictionary<string, List<TextureConfiguration>>();

        public Dictionary<string, TextureConfiguration> imageReplacements = new Dictionary<string, TextureConfiguration>();

        public Dictionary<string, List<MaterialConfiguration>> materialColors = new Dictionary<string, List<MaterialConfiguration>>();

        public Dictionary<string, MaterialConfiguration> materialReplacements = new Dictionary<string, MaterialConfiguration>();

        private WWW toWWW(string texturePath)
        {
            return new WWW(("file:///" + texturePath).Replace(" ", "%20").Replace("\\", "/"));
        }

        private Color32 toColor(string colorText)
        {
            string[] colorBytes = colorText.Split(',');
            return new Color32(
                        byte.Parse(colorBytes[0]),
                        byte.Parse(colorBytes[1]),
                        byte.Parse(colorBytes[2]),
                        byte.Parse(colorBytes[3])
                );
        }

        private void Log(string logtext)
        {
            File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", logtext + "\n");
        }

        private void createOrAdd<T>(ref Dictionary<string, List<T>> dictionary, string key, T config)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<T>());
            }
            dictionary.GetValueSafe(key).Add(config);
        }

        private void LoadTextureConfigurationsFromPack(string packPath)
        {
            if (File.Exists(packPath + "/Parts Texture Replacer.conf"))
            {
                string[] textureConfigurations = File.ReadAllLines(packPath + "/Parts Texture Replacer.conf");
                foreach (string textureConfigLine in textureConfigurations)
                {
                    string[] textureConfig = textureConfigLine.Split('|');
                    if (textureConfig.Length == 2)
                    {
                        createOrAdd<TextureConfiguration>(ref textureReplacements, textureConfig[0], new TextureConfiguration(toWWW(packPath + "/" + textureConfig[1])));
                        Log("Added replacement configuration for " + textureConfig[0] + " => " + packPath + "/" + textureConfig[1]);
                    }
                    else if (textureConfig.Length == 3)
                    {
                        createOrAdd<TextureConfiguration>(ref textureReplacements, textureConfig[0], new TextureConfiguration(textureConfig[1], toWWW(packPath + "/" + textureConfig[2])));
                        Log("Added replacement configuration for " + textureConfig[0] + " => " + textureConfig[1] + " => " + packPath + "/" + textureConfig[2]);
                    }
                    else
                    {
                        Log("Invalid replacement configuration: " + textureConfigLine);
                    }
                }
            }
        }

        private void LoadImageConfigurationsFromPack(string packPath)
        {
            if (File.Exists(packPath + "/Image Replacer.conf"))
            {
                string[] imageConfigurations = File.ReadAllLines(packPath + "/Image Replacer.conf");
                foreach (string imageConfigLine in imageConfigurations)
                {
                    string[] imageConfig = imageConfigLine.Split('|');
                    if (imageConfig.Length == 2)
                    {
                        imageReplacements.Add(imageConfig[0], new TextureConfiguration(toWWW(packPath + "/" + imageConfig[1])));
                        Log("Added replacement configuration for " + imageConfig[0] + " => " + packPath + "/" + imageConfig[1]);
                    }
                    else if (imageConfig.Length == 3)
                    {
                        imageReplacements.Add(imageConfig[0], new TextureConfiguration(imageConfig[1], toWWW(packPath + "/" + imageConfig[2])));
                        Log("Added replacement configuration for " + imageConfig[0] + " => " + imageConfig[1] + " => " + packPath + "/" + imageConfig[2]);
                    }
                    else
                    {
                        Log("Invalid replacement configuration: " + imageConfigLine);
                    }
                }
            }
            
        }

        private void LoadMaterialConfigurationsFromPack(string packPath)
        {
            if (File.Exists(packPath + "/Material Colors.conf"))
            {
                string[] materialConfigurations = File.ReadAllLines(packPath + "/Material Colors.conf");
                foreach (string materialConfiguration in materialConfigurations)
                {
                    string[] materialConfig = materialConfiguration.Split('|');
                    if (materialConfig.Length == 2)
                    {
                        createOrAdd<MaterialConfiguration>(ref materialColors, materialConfig[0], new MaterialConfiguration(toColor(materialConfig[1])));
                        Log("Added color configuration for " + materialConfig[0] + " => " + materialConfig[1]);
                    }
                    else if (materialConfig.Length == 3)
                    {
                        createOrAdd<MaterialConfiguration>(ref materialColors, materialConfig[0], new MaterialConfiguration(materialConfig[1], toColor(materialConfig[2])));
                        Log("Added color configuration for " + materialConfig[0] + " => " + materialConfig[1] + " => " + materialConfig[2]);
                    }
                    else
                    {
                        Log("Invalid color configuration: " + materialConfiguration);
                    }
                }
            }

        }

        private void LoadMaterialAssetBundleFromPack(string packPath)
        {
            if (File.Exists(packPath + "/materials.assetbundle"))
            {
                AssetBundle materialBundle = AssetBundle.LoadFromFile(packPath + "/materials.assetbundle");

                foreach (Material material in materialBundle.LoadAllAssets<Material>())
                {
                    materialReplacements.Add(material.name, new MaterialConfiguration(material));
                    Log("Added replacement configuration for " + material.name + " => " + material);
                }
            }
        }

        private ConfigHolder()
        {
            File.Delete(ModloaderMod.Instance.Modpath + "/replacer.log");

            foreach (string packPath in Directory.GetDirectories(ModloaderMod.Instance.Modpath))
            {
                Log("Loading pack from " + packPath);
                LoadTextureConfigurationsFromPack(packPath);
                LoadImageConfigurationsFromPack(packPath);
                LoadMaterialConfigurationsFromPack(packPath);
                LoadMaterialAssetBundleFromPack(packPath);
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
                        bool replacedIDSpecific = false;
                        foreach (TextureConfiguration config in ConfigHolder.Instance.textureReplacements.GetValueSafe(renderer.material.mainTexture.name))
                        {
                            if (config.PartID == null && !replacedIDSpecific)
                            {
                                config.TexturePath.LoadImageIntoTexture(renderer.material.mainTexture as Texture2D);
                            }
                            else if (config.PartID.Equals(__result.name))
                            {
                                config.TexturePath.LoadImageIntoTexture(renderer.material.mainTexture as Texture2D);
                                replacedIDSpecific = true;
                            }
                        }
                    }
                }
                foreach (Material material in renderer.materials)
                {
                    string materialName = material.name.Replace("(Instance)", "").Trim();
                    if (ConfigHolder.Instance.materialColors.ContainsKey(materialName))
                    {
                        bool replacedIDSpecific = false;
                        foreach (MaterialConfiguration config in ConfigHolder.Instance.materialColors.GetValueSafe(materialName))
                        {
                            if (config.PartID == null && !replacedIDSpecific)
                            {
                                material.color = config.Color;
                            }
                            else if (config.PartID.Equals(__result.name))
                            {
                                material.color = config.Color;
                                replacedIDSpecific = true;
                            }
                        }
                    }
                    if (ConfigHolder.Instance.materialReplacements.ContainsKey(materialName))
                    {
                        material.CopyPropertiesFromMaterial(ConfigHolder.Instance.materialReplacements.GetValueSafe(materialName).Material);
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
                    ConfigHolder.Instance.imageReplacements.GetValueSafe(__result.texture.name).TexturePath.LoadImageIntoTexture(__result.texture as Texture2D);
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
                            ConfigHolder.Instance.imageReplacements.GetValueSafe(material.mainTexture.name).TexturePath.LoadImageIntoTexture(material.mainTexture as Texture2D);
                        }

                    }
                    string materialName = material.name.Replace("(Instance)", "").Trim();
                    if (ConfigHolder.Instance.materialColors.ContainsKey(materialName))
                    {
                        foreach (MaterialConfiguration config in ConfigHolder.Instance.materialColors.GetValueSafe(materialName))
                        {
                            if (config.PartID == null)
                            {
                                material.color = config.Color;
                                break;
                            }
                        }
                    }
                    if (ConfigHolder.Instance.materialReplacements.ContainsKey(materialName))
                    {
                        material.CopyPropertiesFromMaterial(ConfigHolder.Instance.materialReplacements.GetValueSafe(materialName).Material);
                    }

                }
            }
        }
    }
}
