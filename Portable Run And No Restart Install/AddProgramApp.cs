extern alias PRANRIUtils;

using Harmony;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PRANRIUtils.Utils;
using System;

namespace Portable_Run_And_No_Restart_Install
{
    class AddProgramAppLogic
    {

        private AddProgramApp addProgramApp;

        public Button portableButton;

        public int addMode;

        private static Dictionary<AddProgramApp, AddProgramAppLogic> addProgramAppLogicInstances = new Dictionary<AddProgramApp, AddProgramAppLogic>();

        public static AddProgramAppLogic InstanceFor(AddProgramApp addProgramApp)
        {
            AddProgramAppLogic instance = null;
            if (!addProgramAppLogicInstances.ContainsKey(addProgramApp))
            {
                instance = new AddProgramAppLogic(addProgramApp);
                addProgramAppLogicInstances.Add(addProgramApp, instance);
                return instance;
            }
            addProgramAppLogicInstances.TryGetValue(addProgramApp, out instance);
            return instance;
        }

        private AddProgramAppLogic(AddProgramApp addProgramApp) {
            this.addProgramApp = addProgramApp;
        }

        public void Init()
        {
            ChangeUI();
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
            portableButton = UIUtil.CreateTemplateButton(addProgramApp.m_addButton, "Portable", 0f, 0f, -(addProgramApp.m_removeButton.GetComponent<RectTransform>().rect.width + 10f), 0f);
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

        static IEnumerator Postfix(IEnumerator __result, AddProgramApp __instance, OSProgramDesc desc, bool ___m_dayEnded)
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
            var os = __instance.GetComponentInParent<OS>();
            __instance.m_installPopup.SetActive(false);
            OSLogic.InstanceFor(os).AddProgram(desc.m_id);
            OSLogic.InstanceFor(os).UpdatePrograms();
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

        static IEnumerator Postfix(IEnumerator __result, AddProgramApp __instance, OSProgramDesc desc, bool ___m_dayEnded)
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
            var os = __instance.GetComponentInParent<OS>();
            OSLogic.InstanceFor(os).RemoveProgram(desc.m_id);
            OSLogic.InstanceFor(os).UpdatePrograms();
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

        static void Postfix(AddProgramApp __instance, bool add)
        {
            int intMode = add ? 2 : 3;
            AddProgramAppLogic.InstanceFor(__instance).SetMode(intMode);
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
            if (AddProgramAppLogic.InstanceFor(__instance).addMode == 1)
            {
                if (!isUsbDriveInserted)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.INSERT_USB });
                    return;
                }

                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (CareerStatus.Get().IsProgramAvailableForInstall(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, false, delegate
                        {
                            __instance.GetComponentInParent<OS>().Launch(prog);
                        });
                    }
                }
                return;
            }
            if (AddProgramAppLogic.InstanceFor(__instance).addMode == 2)
            {
                if (!isUsbDriveInserted)
                {
                    ReflectionUtils.Run("SetMessage", __instance, new object[] { ScriptLocalization.AddPrograms.INSERT_USB });
                    return;
                }
                bool programsMissing = false;

                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (!computer.m_software.m_programs.Contains(prog.m_id) && CareerStatus.Get().IsProgramAvailableForInstall(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, false, delegate
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
            else if (AddProgramAppLogic.InstanceFor(__instance).addMode == 3)
            {
                bool uninstallablePrograms = false;
                foreach (OSProgramDesc prog in PartsDatabase.GetAllPrograms())
                {
                    if (prog.m_canBeUninstalled && computer.m_software.m_programs.Contains(prog.m_id))
                    {
                        UnityEngine.Object.Instantiate<ProgramIcon>(__instance.programIconPrefab, __instance.m_programList.content).Init(prog, false, delegate
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
