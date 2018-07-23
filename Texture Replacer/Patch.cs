using Harmony;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Texture_Replacer
{
    [HarmonyPatch(typeof(PartsDatabase))]
    [HarmonyPatch("InstantiatePart")]
    class PatchPartTextures
    {
        static GameObject Postfix(GameObject __result)
        {
            string[] textureConfigurations = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Parts Texture Replacer.conf");
            MeshRenderer[] meshRenderers = __result.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer.material.mainTexture != null && typeof(Texture2D).IsInstanceOfType(meshRenderer.material.mainTexture))
                {
                    foreach (string textureConfigLine in textureConfigurations)
                    {
                        string[] textureConfig = textureConfigLine.Split('|');
                        if (meshRenderer.material.mainTexture.name.Equals(textureConfig[0], StringComparison.OrdinalIgnoreCase))
                        {
                            WWW www = new WWW(("file:///" + ModloaderMod.Instance.Modpath + "/" + textureConfig[1]).Replace(" ", "%20").Replace("\\", "/"));
                            www.LoadImageIntoTexture(meshRenderer.material.mainTexture as Texture2D);
                        }
                    }
                }
            }
            return __result;
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Object))]
    [HarmonyPatch("Instantiate")]
    [HarmonyPatch(new Type[] { typeof(MonoBehaviour) })]
    class PatchOthers
    {
        static MonoBehaviour Postfix(MonoBehaviour __result)
        {
            string[] textureConfigurations = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Sprite Texture Replacer.conf");

            Image[] images = __result.gameObject.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.sprite != null && image.sprite.texture != null)
                {
                    File.AppendAllText(ModloaderMod.Instance.Modpath + "/test.log", image.sprite.texture.name + "\n");
                    foreach (string textureConfigLine in textureConfigurations)
                    {
                        string[] textureConfig = textureConfigLine.Split('|');
                        if (image.sprite.texture.name.Equals(textureConfig[0], StringComparison.OrdinalIgnoreCase))
                        {
                            WWW www = new WWW(("file:///" + ModloaderMod.Instance.Modpath + "/" + textureConfig[1]).Replace(" ", "%20").Replace("\\", "/"));
                            www.LoadImageIntoTexture(image.sprite.texture);
                        }
                    }
                }
            }
            return __result;
        }
    }

    //[HarmonyPatch(typeof(SaveLoadSystem))]
    //[HarmonyPatch("LoadSaveGame")]
    //class PatchOthers
    //{
    //    static void Postfix()
    //    {
    //        string[] textureConfigurations = File.ReadAllLines(PCBSModloader.ModLoader.PatchesPath + "/Texture Replacer/Sprite Texture Replacer.conf");
    //        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    //        foreach (GameObject go in rootGameObjects)
    //        {
    //            Texture2D[] test1 = go.GetComponentsInChildren<Texture2D>(true);
    //            foreach (Texture2D texture in test1)
    //            {
    //                if (texture != null)
    //                {
    //                    File.AppendAllText(PCBSModloader.ModLoader.PatchesPath + "/Texture Replacer/testtex.log", texture.name + "\n");
    //                }
    //            }
    //            Image[] images = go.GetComponentsInChildren<Image>(true);
    //            foreach (Image image in images)
    //            {
    //                if (image.sprite != null && image.sprite.texture != null)
    //                {
    //                    File.AppendAllText(PCBSModloader.ModLoader.PatchesPath + "/Texture Replacer/test.log", image.sprite.texture.name + "\n");
    //                    foreach (string textureConfigLine in textureConfigurations)
    //                    {
    //                        string[] textureConfig = textureConfigLine.Split('|');
    //                        if (image.sprite.texture.name.Equals(textureConfig[0], StringComparison.OrdinalIgnoreCase))
    //                        {
    //                            WWW www = new WWW(("file:///" + PCBSModloader.ModLoader.PatchesPath + "/Texture Replacer/" + textureConfig[1]).Replace(" ", "%20").Replace("\\", "/"));
    //                            www.LoadImageIntoTexture(image.sprite.texture);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}



}
