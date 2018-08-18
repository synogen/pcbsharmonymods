using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texture_And_Material_Replacer
{
    class State
    {
        private static State singletonInstance;

        public static State Instance
        {
            get
            {
                singletonInstance = singletonInstance != null ? singletonInstance : new State();
                return singletonInstance;
            }
        }

        public bool updateMessage = false;
    }
}
