using Harmony;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Asset_Replacer
{

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
                                renderer.material.mainTexture = config.Texture;
                            }
                            else if (config.PartID.Equals(__result.name))
                            {
                                renderer.material.mainTexture = config.Texture;
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
        static void Postfix(Sprite __result)
        {
            if (__result != null && __result.texture != null)
            {
                if (ConfigHolder.Instance.imageReplacements.ContainsKey(__result.texture.name))
                {
                    TextureConfiguration config = ConfigHolder.Instance.imageReplacements[__result.texture.name];
                    if (!config.replaced)
                    {
                        // threading in unity sucks unless .NET 4.6 experimental compatibility is used so this can't be loaded in a seperate thread right now
                        // at least not for PCBS
                        config.www.LoadImageIntoTexture(__result.texture);
                        config.replaced = true;
                    }
                    
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
            State.Instance.updateMessage = false;
            foreach (Renderer o in UnityEngine.Object.FindObjectsOfType<Renderer>())
            {
                foreach (Material material in o.materials)
                {
                    if (material.mainTexture != null)
                    {
                        if (ConfigHolder.Instance.imageReplacements.ContainsKey(material.mainTexture.name))
                        {
                            material.mainTexture = ConfigHolder.Instance.imageReplacements.GetValueSafe(material.mainTexture.name).Texture;
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

    [HarmonyPatch(typeof(SoundPlayer))]
    [HarmonyPatch("PlaySoundAtSource")]
    [HarmonyPatch(new Type[] { typeof(AudioSource), typeof(Transform) })]
    class PatchSounds
    {
        static void Prefix(ref AudioSource sound)
        {
            if (sound != null && sound.clip != null)
            {
                if (ConfigHolder.Instance.soundReplacements.ContainsKey(sound.clip.name))
                {
                    sound.clip = ConfigHolder.Instance.soundReplacements.GetValueSafe(sound.clip.name).Clip;
                }
            }
        }
    }

}
