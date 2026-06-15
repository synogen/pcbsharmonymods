using HarmonyLib;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Portable_Run_And_No_Restart_Bepinex
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
            if (addProgramAppLogicInstances.TryGetValue(addProgramApp, out instance))
            {
                return instance;
            }
            instance = new AddProgramAppLogic(addProgramApp);
            addProgramAppLogicInstances.Add(addProgramApp, instance);
            return instance;
        }

        public static void Cleanup(AddProgramApp addProgramApp)
        {
            addProgramAppLogicInstances.Remove(addProgramApp);
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
            RectTransform addRT = addProgramApp.m_addButton.GetComponent<RectTransform>();
            RectTransform removeRT = addProgramApp.m_removeButton.GetComponent<RectTransform>();

            float addWidth = addRT.rect.width;
            float removeWidth = removeRT.rect.width;
            float spacing = 5f;
            float totalWidth = addWidth + removeWidth + spacing;
            float newWidth = (totalWidth - spacing * 2f) / 3f;
            float delta = (addWidth - newWidth + removeWidth - newWidth) / 2f;

            Vector3 addOrigPos = addRT.localPosition;
            Vector3 removeOrigPos = removeRT.localPosition;

            addRT.sizeDelta = new Vector2(addRT.sizeDelta.x - delta, addRT.sizeDelta.y);
            addRT.localPosition = new Vector3(addOrigPos.x + delta * 2f, addOrigPos.y, addOrigPos.z);

            removeRT.sizeDelta = new Vector2(removeRT.sizeDelta.x - delta, removeRT.sizeDelta.y);
            removeRT.localPosition = new Vector3(removeOrigPos.x + delta, removeOrigPos.y, removeOrigPos.z);

            addProgramApp.m_addButton.GetComponentInChildren<Text>().text = "Add";
            addProgramApp.m_removeButton.GetComponentInChildren<Text>().text = "Remove";

            portableButton = UIUtil.CreateTemplateButton(addProgramApp.m_addButton, "Portable", 0f, 0f, -(newWidth + spacing), 0f);
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

            __instance.m_message.gameObject.SetActive(false);
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
                    if (prog.m_id != "ADDPROGRAM" && CareerStatus.Get().IsProgramAvailableForInstall(prog.m_id))
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
                    if (!computer.m_software.m_programs.Contains(prog.m_id) && CareerStatus.Get().IsProgramAvailableForInstall(prog.m_id) && prog.m_canBeInstalled)
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
