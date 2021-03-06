﻿using System;
using System.Reflection;

namespace Utils
{
    public class ReflectionUtils
    {
        public static T Get<T>(string varname, System.Object instance)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(varname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)field.GetValue(instance);
        }

        public static void Set<T>(string varname, System.Object instance, T value)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(varname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            field.SetValue(instance, value);
        }

        public static void Run(string methodname, System.Object instance)
        {
            Run(methodname, instance, new object[] { });
        }

        public static void Run(string methodname, System.Object instance, object[] parameters)
        {
            Type shop = instance.GetType();
            MethodInfo method = shop.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            method.Invoke(instance, parameters);
        }


        public static T Run<T>(string methodname, System.Object instance, object[] parameters)
        {
            Type shop = instance.GetType();
            MethodInfo method = shop.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)method.Invoke(instance, parameters);
        }
    }
}
