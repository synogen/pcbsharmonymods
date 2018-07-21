using System;
using System.Reflection;

namespace Utils
{
    public class ReflectionUtils
    {
        public static T Get<T>(string varname, System.Object instance)
        {
            Type type = instance.GetType();
            FieldInfo field = type.GetField(varname, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(instance);
        }

        public static void Run(string methodname, System.Object instance)
        {
            Run(methodname, instance, new object[] { });
        }

        public static void Run(string methodname, System.Object instance, object[] parameters)
        {
            Type shop = instance.GetType();
            MethodInfo method = shop.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(instance, parameters);
        }


        public static T Run<T>(string methodname, System.Object instance, object[] parameters)
        {
            Type shop = instance.GetType();
            MethodInfo method = shop.GetMethod(methodname, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(instance, parameters);
        }
    }
}
