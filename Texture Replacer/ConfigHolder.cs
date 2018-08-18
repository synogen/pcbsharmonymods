using Harmony;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
            File.AppendAllText(ModloaderMod.Instance.Modpath + "/replacer.log", logtext + "\r\n");
        }

        private void createOrAdd<T>(ref Dictionary<string, List<T>> dictionary, string key, T config)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<T>());
            }
            dictionary.GetValueSafe(key).Add(config);
        }

        private void createIfSafe<T>(ref Dictionary<string, T> dictionary, string key, T config)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, config);
            } else
            {
                Log("Duplicate configuration for " + key + ". Multiple configurations not supported for this kind of replacement.");
            }
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
                        createIfSafe(ref imageReplacements, imageConfig[0], new TextureConfiguration(toWWW(packPath + "/" + imageConfig[1])));
                        Log("Added replacement configuration for " + imageConfig[0] + " => " + packPath + "/" + imageConfig[1]);
                    }
                    else if (imageConfig.Length == 3)
                    {
                        createIfSafe(ref imageReplacements, imageConfig[0], new TextureConfiguration(imageConfig[1], toWWW(packPath + "/" + imageConfig[2])));
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
                    createIfSafe(ref materialReplacements, material.name, new MaterialConfiguration(material));
                    Log("Added replacement configuration for " + material.name + " => " + material);
                }

                materialBundle.Unload(false);
            }
        }

        private ConfigHolder()
        {
            File.Delete(ModloaderMod.Instance.Modpath + "/replacer.log");

            foreach (string packPath in Directory.GetDirectories(ModloaderMod.Instance.Modpath))
            {
                Log("-- Loading pack from " + packPath);
                LoadTextureConfigurationsFromPack(packPath);
                LoadImageConfigurationsFromPack(packPath);
                LoadMaterialConfigurationsFromPack(packPath);
                LoadMaterialAssetBundleFromPack(packPath);
                Log("-- Pack loaded\r\n");
            }

        }

        public void ReloadConfigurations()
        {
            Log("Reloading replacement configurations");
            textureReplacements.Clear();
            imageReplacements.Clear();
            materialColors.Clear();
            materialReplacements.Clear();

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
}
