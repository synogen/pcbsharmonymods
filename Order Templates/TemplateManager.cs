using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Order_Templates
{

    public class TemplateManager
    {

        private TemplateManager()
        {
            this.templates = new Dictionary<string, List<string>>();
            this.LoadContentFromFile();
        }

        private static TemplateManager singletonInstance;

        public static TemplateManager Instance
        {
            get
            {
                if (TemplateManager.singletonInstance == null)
                {
                    TemplateManager.singletonInstance = new TemplateManager();
                }
                return singletonInstance;
            }
        }

 
        public void AddTemplate(string name, List<ShopEntry> list)
        {
            List<string> partIds = new List<string>();
            foreach (ShopEntry shopEntry in list)
            {
                partIds.Add(shopEntry.m_part.m_id);
            }
            this.templates.Add(name, partIds);
            this.SaveContentToFile();
        }


        public void RemoveTemplate(string name)
        {
            this.templates.Remove(name);
            this.SaveContentToFile();
        }


        public List<string> GetTemplate(string name)
        {
            List<string> outEntries;
            this.templates.TryGetValue(name, out outEntries);
            return outEntries;
        }


        public Dictionary<string, List<string>> GetAllTemplates()
        {
            return this.templates;
        }


        private void SaveContentToFile()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this.templates);
            stream.Close();
        }


        private void LoadContentFromFile()
        {
            if (File.Exists(ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                this.templates = (Dictionary<string, List<string>>)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        private Dictionary<string, List<string>> templates;

        private static T DeepClone<T>(T obj)
        {
            T objResult = default(T);
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = (T)bf.Deserialize(ms);
            }
            return objResult;
        }
    }

}
