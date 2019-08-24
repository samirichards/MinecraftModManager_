using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Classes
{
    static class Utilities
    {
        public static Mod GetModFromJson(string temp)
        {
            int start;
            if (temp.IndexOf('[') != 0)
            {
                start = temp.IndexOf('[') - 1;
            }
            else
            {
                start = temp.IndexOf('[');
            }
            int end = temp.LastIndexOf(']') + 1;
            return JsonConvert.DeserializeObject<List<Classes.Mod>>(temp.Substring(start, end - start))[0];
        }
    }
}
