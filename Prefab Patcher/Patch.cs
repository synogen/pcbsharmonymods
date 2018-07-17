using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prefab_Patcher
{
    [HarmonyPatch(typeof(PartsDatabase))]
    [HarmonyPatch("GetPrefab")]
    [HarmonyPatch(new Type[] { typeof(PartDesc) })]
    class PatchPrefabs
    {
        static GameObject Postfix(GameObject __result, PartDesc part)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(PCBSModloader.ModLoader.AssetBundlesPath + "/titanx");

            if ("GPU_GIGABYTE_10".Equals(part.m_id))
            {
                GameObject modded = assetBundle.LoadAsset<GameObject>("assets/prefabs/titan x.prefab");
                modded.AddComponent<BoxCollider>();
                modded.AddComponent<ComponentPC>();
                return modded;
            }
            return __result;
        }
    }
}
