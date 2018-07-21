using Harmony;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Portable_Run_And_No_Restart_Install
{
    class AddProgramAppLogic
    {

        private AddProgramApp addProgramApp;

        public static Button portableButton;

        public static int addMode;

        public AddProgramAppLogic(AddProgramApp addProgramApp)
        {
            this.addProgramApp = addProgramApp;
        }

        private void ChangeUI()
        {   
            RectTransform rt = addProgramApp.m_addButton.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x - 78f, rt.sizeDelta.y);
            RectTransform rt2 = addProgramApp.m_removeButton.GetComponent<RectTransform>();
            rt2.sizeDelta = new Vector2(rt2.sizeDelta.x - 78f, rt2.sizeDelta.y);
            addProgramApp.m_addButton.transform.localPosition = new Vector3(addProgramApp.m_addButton.transform.localPosition.x + 156f, addProgramApp.m_addButton.transform.localPosition.y, addProgramApp.m_addButton.transform.localPosition.z);
            addProgramApp.m_addButton.GetComponentInChildren<Text>().text = "Add";
            addProgramApp.m_removeButton.transform.localPosition = new Vector3(addProgramApp.m_removeButton.transform.localPosition.x + 78f, addProgramApp.m_removeButton.transform.localPosition.y, addProgramApp.m_removeButton.transform.localPosition.z);
            addProgramApp.m_removeButton.GetComponentInChildren<Text>().text = "Remove";
            portableButton = UIUtil.CreateTemplateButton(addProgramApp.m_addButton, "Portable", 0f, 0f, -(addProgramApp.m_addButton.GetComponent<RectTransform>().rect.width + 10f), 0f);
            portableButton.onClick.AddListener(new UnityAction(SetPortableMode));
            SetPortableMode();
        }

        public void SetPortableMode()
        {
            SetMode(1);
        }

        public void SetMode(int mode)
        {
            portableButton.interactable = (mode != 1);
            addProgramApp.m_addButton.interactable = (mode != 2);
            addProgramApp.m_removeButton.interactable = (mode != 3);
            addMode = mode;
            ReflectionUtils.Run("ShowPrograms", addProgramApp);
        }
    }

    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("Install")]
    class AddProgramAppInstallPatch
    {
        static bool Prefix()
        {
            return false;
        }

        static IEnumerator Postfix(AddProgramApp __instance, OSProgramDesc desc, bool ___m_dayEnded, OS ___m_os)
        {
            ___m_dayEnded = false;
            ReflectionUtils.Run("ShowProgressDialog", __instance, new object[] { ScriptLocalization.AddPrograms.INSTALLING, desc });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0f, 0.9f, (float)desc.m_installTime * 0.4f, true });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0.9f, 0.95f, (float)desc.m_installTime * 0.6f, true });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0.95f, 1f, 1f, false });
            ComputerSoftware software = __instance.GetComponentInParent<VirtualComputer>().GetComputer().m_software;
            software.m_programs = new List<string>(software.m_programs)
            {
                desc.m_id
            }.ToArray();
            __instance.m_installPopup.SetActive(false);
            ReflectionUtils.Run("AddProgram", ___m_os, new object[] { desc.m_id });
            ReflectionUtils.Run("UpdatePrograms", ___m_os);
            ReflectionUtils.Run("UpdateProgramList", __instance);

            yield break;
        }
    }

    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("Uninstall")]
    class AddProgramAppUninstallPatch
    {
        static bool Prefix()
        {
            return false;
        }

        static IEnumerator Postfix(AddProgramApp __instance, OSProgramDesc desc, bool ___m_dayEnded, OS ___m_os)
        {
            ___m_dayEnded = false;
            ReflectionUtils.Run("ShowProgressDialog", __instance, new object[] { ScriptLocalization.AddPrograms.REMOVING, desc });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0f, 0.9f, (float)desc.m_removeTime * 0.2f, true });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0.9f, 0.95f, (float)desc.m_removeTime * 0.8f, true });
            yield return ReflectionUtils.Run<IEnumerator>("ShowProgress", __instance, new object[] { 0.95f, 1f, 1f, false });
            ComputerSoftware software = __instance.GetComponentInParent<VirtualComputer>().GetComputer().m_software;
            List<string> list = new List<string>(software.m_programs);
            list.Remove(desc.m_id);
            software.m_programs = list.ToArray();
            __instance.m_installPopup.SetActive(false);
            ReflectionUtils.Run("RemoveProgram", ___m_os, new object[] { desc.m_id });
            ReflectionUtils.Run("UpdatePrograms", ___m_os);
            ReflectionUtils.Run("UpdateProgramList", __instance);

            yield break;
        }
    }

    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("SetMode")]
    class AddProgramAppSetModePatch
    {
        static bool Prefix()
        {
            return false;
        }

        static void Postfix(AddProgramApp __instance, int ___m_addMode, int mode)
        {
            AddProgramAppLogic.portableButton.interactable = (mode != 1);
            __instance.m_addButton.interactable = (mode != 2);
            __instance.m_removeButton.interactable = (mode != 3);
            AddProgramAppLogic.addMode = mode;
            ReflectionUtils.Run("ShowPrograms", __instance);
        }
    }

    [HarmonyPatch(typeof(AddProgramApp))]
    [HarmonyPatch("UpdateProgramList")]
    class AddProgramAppUpdateProgramListPatch
    {
        static bool Prefix()
        {
            return false;
        }

        static void Postfix(AddProgramApp __instance)
        {
            foreach (object o in __instance.m_programList.content)
            {
                UnityEngine.Object.Destroy(((Transform)o).gameObject);
            }

            ComputerSave computer = __instance.GetComponentInParent<VirtualComputer>().GetComputer();
            bool isUsbDriveInserted = computer.IsUSBDriveInserted();
            if (!isUsbDriveInserted)
            {
                WorkStation componentInParent = __instance.GetComponentInParent<WorkStation>();
                if (componentInParent && componentInParent.m_slot && componentInParent.m_slot.m_type == BenchSlot.Type.SHOP_COMPUTER)
                {
                    isUsbDriveInserted = true;
                }
            }
            if (AddProgramAppLogic.addMode == 1)
            {
                if (!isUsbDriveInserted)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.INSERT_USB });
                    return;
                }

                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (CareerStatus.Get().IsProgramUnlocked(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, delegate
                        {
                            ReflectionUtils.Get<OS>("m_os", __instance).Launch(prog);
                        });
                    }
                }
                return;
            }
            if (AddProgramAppLogic.addMode == 2)
            {
                if (!isUsbDriveInserted)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.INSERT_USB });
                    return;
                }
                bool programsMissing = false;

                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (!computer.m_software.m_programs.Contains(prog.m_id) && CareerStatus.Get().IsProgramUnlocked(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, delegate
                        {
                            __instance.StartCoroutine(ReflectionUtils.Run<IEnumerator>("Install", __instance, new object[] { prog }));
                        });
                        programsMissing = true;
                    }
                }
                
                if (!programsMissing)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.ALL_INSTALLED });
                    return;
                }
            }
            else if (AddProgramAppLogic.addMode == 3)
            {
                bool uninstallablePrograms = false;
                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (prog.m_canBeUninstalled && computer.m_software.m_programs.Contains(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, delegate
                        {
                            
                            __instance.StartCoroutine(ReflectionUtils.Run<IEnumerator>("Uninstall", __instance, new object[] { prog }));
                        });
                        uninstallablePrograms = true;
                    }
                } 
                if (!uninstallablePrograms)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.NONE_INSTALLED });
                }
            }
        }
    }
}
