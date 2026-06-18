using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Utils
{
    public static class Json
    {
        public static string Serialize(object obj)
        {
            if (obj == null) return "null";
            if (obj is string s) return "\"" + EscapeString(s) + "\"";
            if (obj is bool b) return b ? "true" : "false";
            if (obj is int || obj is long || obj is short || obj is byte) return obj.ToString();
            if (obj is float || obj is double || obj is decimal) return ((IFormattable)obj).ToString("R", CultureInfo.InvariantCulture);
            if (obj is Enum) return "\"" + obj.ToString() + "\"";

            Type type = obj.GetType();

            if (obj is IDictionary dict)
            {
                var sb = new StringBuilder();
                sb.Append("{");
                bool first = true;
                foreach (DictionaryEntry entry in dict)
                {
                    if (!first) sb.Append(",");
                    sb.Append(Serialize(entry.Key));
                    sb.Append(":");
                    sb.Append(Serialize(entry.Value));
                    first = false;
                }
                sb.Append("}");
                return sb.ToString();
            }

            if (obj is IList list)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(Serialize(list[i]));
                }
                sb.Append("]");
                return sb.ToString();
            }

            return "{}";
        }

        public static T Deserialize<T>(string json)
        {
            return (T)DeserializeValue(json.Trim(), typeof(T));
        }

        private static object DeserializeValue(string json, Type targetType)
        {
            if (string.IsNullOrEmpty(json) || json == "null") return null;

            if (targetType == typeof(string))
                return UnescapeString(TrimQuotes(json));

            if (targetType == typeof(bool))
                return json == "true";

            if (targetType == typeof(int))
                return int.Parse(json, CultureInfo.InvariantCulture);

            if (targetType == typeof(float))
                return float.Parse(json, CultureInfo.InvariantCulture);

            if (targetType == typeof(double))
                return double.Parse(json, CultureInfo.InvariantCulture);

            if (targetType.IsEnum)
            {
                string enumStr = TrimQuotes(json);
                try
                {
                    return Enum.Parse(targetType, enumStr, true);
                }
                catch
                {
                    return Enum.ToObject(targetType, int.Parse(json, CultureInfo.InvariantCulture));
                }
            }

            if (targetType.IsGenericType)
            {
                Type genDef = targetType.GetGenericTypeDefinition();

                if (genDef == typeof(Dictionary<,>))
                {
                    Type keyType = targetType.GetGenericArguments()[0];
                    Type valType = targetType.GetGenericArguments()[1];
                    return DeserializeDictionary(json, keyType, valType);
                }

                if (genDef == typeof(List<>) || genDef == typeof(IList<>))
                {
                    Type itemType = targetType.GetGenericArguments()[0];
                    return DeserializeList(json, itemType);
                }
            }

            if (targetType.IsArray)
            {
                Type itemType = targetType.GetElementType();
                return DeserializeArray(json, itemType);
            }

            return null;
        }

        private static object DeserializeDictionary(string json, Type keyType, Type valType)
        {
            var dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valType));
            json = json.Trim().TrimStart('{').TrimEnd('}');

            if (string.IsNullOrEmpty(json)) return dict;

            foreach (var kvPair in SplitTopLevel(json, ','))
            {
                var parts = SplitTopLevel(kvPair, ':');
                if (parts.Count == 2)
                {
                    object key = DeserializeValue(parts[0].Trim(), keyType);
                    object val = DeserializeValue(parts[1].Trim(), valType);
                    dict.Add(key, val);
                }
            }

            return dict;
        }

        private static object DeserializeList(string json, Type itemType)
        {
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
            json = json.Trim().TrimStart('[').TrimEnd(']');

            if (string.IsNullOrEmpty(json)) return list;

            foreach (var item in SplitTopLevel(json, ','))
            {
                list.Add(DeserializeValue(item.Trim(), itemType));
            }

            return list;
        }

        private static object DeserializeArray(string json, Type itemType)
        {
            var list = DeserializeList(json, itemType);
            var arr = Array.CreateInstance(itemType, ((IList)list).Count);
            ((IList)list).CopyTo(arr, 0);
            return arr;
        }

        private static List<string> SplitTopLevel(string input, char delimiter)
        {
            var result = new List<string>();
            int depth = 0;
            bool inString = false;
            var current = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '\\' && inString)
                {
                    current.Append(c);
                    if (i + 1 < input.Length) current.Append(input[++i]);
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                    current.Append(c);
                    continue;
                }

                if (inString)
                {
                    current.Append(c);
                    continue;
                }

                if (c == '{' || c == '[' || c == '(') depth++;
                else if (c == '}' || c == ']' || c == ')') depth--;

                if (c == delimiter && depth == 0)
                {
                    result.Add(current.ToString());
                    current.Length = 0;
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0) result.Add(current.ToString());
            return result;
        }

        private static string TrimQuotes(string s)
        {
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
                return s.Substring(1, s.Length - 2);
            return s;
        }

        private static string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }

        private static string UnescapeString(string s)
        {
            return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
        }
    }
}
