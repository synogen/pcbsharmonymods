extern alias PRANRIUtils;

using System;
using System.Collections.Generic;
using UnityEngine;
using PRANRIUtils.Utils;

namespace Portable_Run_And_No_Restart_Install
{
    class OSLogic
    {
        private OS os;

        private static Dictionary<OS, OSLogic> OSLogicInstances = new Dictionary<OS, OSLogic>();

        public static OSLogic InstanceFor(OS os)
        {
            OSLogic instance = null;
            if (!OSLogicInstances.ContainsKey(os))
            {
                instance = new OSLogic(os);
                OSLogicInstances.Add(os, instance);
                return instance;
            }
            OSLogicInstances.TryGetValue(os, out instance);
            return instance;
        }

        private OSLogic(OS os)
        {
            this.os = os;
        }

        public void Init()
        {
        }

        internal void AddProgram(string program)
        {
            string[] programsInstalled = ReflectionUtils.Get<string[]>("m_programsInstalled", os);
            Array.Resize<string>(ref programsInstalled, programsInstalled.Length + 1);
            programsInstalled[programsInstalled.Length - 1] = program;
            ReflectionUtils.Set<string[]>("m_programsInstalled", os, programsInstalled);
        }


        internal void RemoveProgram(string program)
        {
            string[] programsInstalled = ReflectionUtils.Get<string[]>("m_programsInstalled", os);
            string[] programs = new string[programsInstalled.Length - 1];
            int index = Array.IndexOf<string>(programsInstalled, program);
            if (index > 0)
            {
                Array.Copy(programsInstalled, 0, programs, 0, index);
            }
            if (index < programsInstalled.Length - 1)
            {
                Array.Copy(programsInstalled, index + 1, programs, index, programsInstalled.Length - index - 1);
            }
            Array.Resize<string>(ref programsInstalled, programsInstalled.Length - 1);
            ReflectionUtils.Set<string[]>("m_programsInstalled", os, programs);
        }


        internal void UpdatePrograms() // TODO check how OS/Desktop does this in PCBS and adapt accordingly or call directly if at all possible
        {
            foreach (ProgramIcon programIcon in os.transform.GetComponentsInChildren<ProgramIcon>())
            {
                if (programIcon.transform.parent == os.transform)
                {
                    programIcon.transform.parent = null;
                    UnityEngine.Object.Destroy(programIcon);
                }
            }
            string[] programsInstalled = ReflectionUtils.Get<string[]>("m_programsInstalled", os);
            List<ProgramIcon> programIcons = ReflectionUtils.Get<List<ProgramIcon>>("m_icons", os);
            programIcons.Clear();
            float num = 100f;
            Rect rect = (os.transform as RectTransform).rect;
            var spacing = 100f;
            float num2 = rect.height - spacing;
            foreach (OSProgramDesc program in PartsDatabase.GetAllPrograms())
            {
                if (Array.Find<string>(programsInstalled, (string p) => p == program.m_id) != null)
                {
                    ProgramIcon programIcon2 = UnityEngine.Object.Instantiate<ProgramIcon>(os.m_programIconPrefab);
                    programIcon2.Init(program, false, null);
                    programIcon2.transform.SetParent(os.transform, false);
                    programIcon2.transform.localPosition = new Vector3(num, num2);
                    programIcons.Add(programIcon2);
                    num2 -= spacing;
                    if (num2 - spacing < 0f)
                    {
                        num += spacing;
                        num2 = rect.height - spacing;
                    }
                }
            }
        }
    }
}
