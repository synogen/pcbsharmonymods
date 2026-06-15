using System;
using System.Reflection;
using UnityEngine;

namespace Utils
{
    public class ReflectionUtils
    {
        public static T Get<T>(string varname, System.Object instance)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(varname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                Debug.LogError($"[ReflectionUtils] Field '{varname}' not found on type '{type.FullName}'");
                return default(T);
            }
            return (T)field.GetValue(instance);
        }

        public static void Set<T>(string varname, System.Object instance, T value)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(varname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                Debug.LogError($"[ReflectionUtils] Field '{varname}' not found on type '{type.FullName}'");
                return;
            }
            field.SetValue(instance, value);
        }

        public static void Run(string methodname, System.Object instance)
        {
            Run(methodname, instance, new object[] { });
        }

        public static void Run(string methodname, System.Object instance, object[] parameters)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                Debug.LogError($"[ReflectionUtils] Method '{methodname}' not found on type '{type.FullName}'");
                return;
            }
            method.Invoke(instance, parameters);
        }


        public static T Run<T>(string methodname, System.Object instance, object[] parameters)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                Debug.LogError($"[ReflectionUtils] Method '{methodname}' not found on type '{type.FullName}'");
                return default(T);
            }
            return (T)method.Invoke(instance, parameters);
        }
    }
}
