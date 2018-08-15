using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utils
{
    public class ConfigUtil
    {
        public static void SaveContentToJson<T>(T content, string filepath)
        {
            string json = JsonConvert.SerializeObject(content);
            File.WriteAllText(filepath, json);
        }

        public static T LoadContentFromJson<T>(string filepath)
        {
            if (File.Exists(filepath))
            {
                string json = File.ReadAllText(filepath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }


        public static void SaveContentToFile<T>(T content, string filepath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, content);
            stream.Close();
        }


        public static T LoadContentFromFile<T>(string filepath)
        {
            T result = default(T);
            if (File.Exists(filepath))
            {

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                Stream stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                result = (T)formatter.Deserialize(stream);
                stream.Close();
            }
            return result;
        }
    }
}
