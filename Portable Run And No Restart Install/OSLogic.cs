using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Portable_Run_And_No_Restart_Install
{
    class OSLogic
    {
        private OS os;

        public OSLogic(OS os)
        {
            this.os = os;
        }

        internal void AddProgram(string program)
        {
            string[] programsInstalled = Get<string[]>("m_programsInstalled", os);
            Array.Resize<string>(ref programsInstalled, programsInstalled.Length + 1);
            programsInstalled[programsInstalled.Length - 1] = program;
        }

        private T Get<T>(string varname, System.Object instance)
        {
            Type shopType = typeof(OS);
            FieldInfo field = shopType.GetField(varname, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(os);
        }

        internal void RemoveProgram(string program)
        {
            string[] programsInstalled = Get<string[]>("m_programsInstalled", os);
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
            Array.Copy(programs, programsInstalled, programsInstalled.Length);
        }


        internal void UpdatePrograms()
        {
            foreach (ProgramIcon programIcon in os.transform.GetComponentsInChildren<ProgramIcon>())
            {
                if (programIcon.transform.parent == os.transform)
                {
                    programIcon.transform.parent = null;
                    UnityEngine.Object.Destroy(programIcon);
                }
            }
            string[] programsInstalled = Get<string[]>("m_programsInstalled", os);
            List<ProgramIcon> programIcons = Get<List<ProgramIcon>>("m_icons", os);
            programIcons.Clear();
            float num = 100f;
            Rect rect = (os.transform as RectTransform).rect;
            float num2 = rect.height - os.m_iconSpacing;
            foreach (OSProgramDesc program in PartsDatabase.GetAllPrograms())
            {
                if (Array.Find<string>(programsInstalled, (string p) => p == program.m_id) != null)
                {
                    ProgramIcon programIcon2 = UnityEngine.Object.Instantiate<ProgramIcon>(os.m_programIconPrefab);
                    programIcon2.Init(program, null);
                    programIcon2.transform.SetParent(os.transform, false);
                    programIcon2.transform.localPosition = new Vector3(num, num2);
                    programIcons.Add(programIcon2);
                    num2 -= os.m_iconSpacing;
                    if (num2 - os.m_iconSpacing < 0f)
                    {
                        num += os.m_iconSpacing;
                        num2 = rect.height - os.m_iconSpacing;
                    }
                }
            }
        }
    }
}
