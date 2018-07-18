using Harmony;

namespace Show_Customer_Specs
{
    [HarmonyPatch(typeof(Job))]
    [HarmonyPatch("GetBody")]
    class Patch
    {
        static string Postfix(string __result, Job __instance)
        {
            string specs = "";
            if (
                __instance.GetJobType() == JobDesc.Type.DIAGNOSE ||
                __instance.GetJobType() == JobDesc.Type.FIX ||
                __instance.GetJobType() == JobDesc.Type.UPGRADE
            )
            {
                specs = "\n\n<b>Current specs:</b>\n";
                foreach (PartInstance part in __instance.GetComputer().GetAllParts())
                {
                    switch (part.GetPart().m_type)
                    {
                        case PartDesc.Type.MOTHERBOARD:
                        case PartDesc.Type.GPU:
                        case PartDesc.Type.HDD:
                        case PartDesc.Type.RAM:
                        case PartDesc.Type.CPU:
                        case PartDesc.Type.SSD:
                        case PartDesc.Type.PSU:
                            string type = part.GetPart().m_type == PartDesc.Type.MOTHERBOARD ? "MB" : part.GetPart().m_type.ToString();
                            specs = specs + type + ":  \t" + part.GetPart().m_uiName + "\n";
                            break;
                    }
                }
            }
            return __result + specs;
        }
    }
}
