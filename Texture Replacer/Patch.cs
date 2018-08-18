using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace Texture_And_Material_Replacer
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
            State.Instance.updateMessage = false;
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
