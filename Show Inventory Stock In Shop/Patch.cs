using Harmony;
using System;
using System.IO;
using UnityEngine;

namespace Show_Inventory_Stock_In_Shop
{
    [HarmonyPatch(typeof(PartDesc))]
    [HarmonyPatch("m_uiLongSpec", PropertyMethod.Getter)]
    class Patch
    {
        static string Postfix(string __result, PartDesc __instance)
        {

            if (__instance.m_id != null)
            {
                int i = 0;
                // check against all inventory parts
                foreach (PartInstance part in CareerStatus.Get().GetInventory())
                {
                    if (part.GetPartId() != null && __instance.m_id.Equals(part.GetPartId()))
                    {
                        i++;
                    }
                }

                // check against all stored computers
                SaveLoadSystem slsys = WorkshopController.Get().slsys;
                foreach (ComputerSave computer in slsys.localSaveGame.computersInStorage)
                {
                    if (computer.caseID != null && __instance.m_id.Equals(computer.caseID.GetPartId()))
                    {
                        i++;
                    }
                }
                // check against all bench computers
                foreach (ComputerSave computer in slsys.localSaveGame.computersOnBenches)
                {
                    if (computer.caseID != null && __instance.m_id.Equals(computer.caseID.GetPartId()))
                    {
                        i++;
                    }

                }

                float max = 10f;
                if (File.Exists(ModloaderMod.Instance.Modpath + "/Show Inventory Stock In Shop.conf"))
                {
                    string[] config = File.ReadAllLines(ModloaderMod.Instance.Modpath + "/Show Inventory Stock In Shop.conf");
                    max = float.Parse(config[0].Split('=')[1]);
                }

                Color32 color = Color.Lerp(Color.red, Color.green, (Convert.ToSingle(i) / max) > 1f ? 1f : Convert.ToSingle(i) / max);
                string colorString = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
                return "<b><color=" + colorString + ">[ " + i + " in inventory ]</color></b>\n" + __result;
            }
            return __result;
        }
    }
}
