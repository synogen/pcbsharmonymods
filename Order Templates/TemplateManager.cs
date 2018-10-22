using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Utils;

namespace Order_Templates
{

    public class TemplateManager
    {

        private TemplateManager()
        {
            templates = ConfigUtil.LoadContentFromFile<Dictionary<string, List<string>>>(ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin");
            if (templates == null)
            {
                this.templates = new Dictionary<string, List<string>>();
            }
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
            ConfigUtil.SaveContentToFile(templates, ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin");
        }


        public void RemoveTemplate(string name)
        {
            this.templates.Remove(name);
            ConfigUtil.SaveContentToFile(templates, ModloaderMod.Instance.Modpath + "/shoppingTemplates.bin");
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
