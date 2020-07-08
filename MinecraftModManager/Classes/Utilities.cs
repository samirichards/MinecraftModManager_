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
            return JsonConvert.DeserializeObject<List<Mod>>(temp.Substring(start, end - start))[0];
        }

        public static bool IsVersionLater(this string v1, string v2)
        {
            // split into groups
            string[] cur = v1.Split('.');
            string[] cmp = v2.Split('.');
            // get max length and fill the shorter one with zeros
            int len = Math.Max(cur.Length, cmp.Length);
            int[] vs = new int[len];
            int[] cs = new int[len];
            Array.Clear(vs, 0, len);
            Array.Clear(cs, 0, len);
            int idx = 0;
            // skip non digits
            foreach (string n in cur)
            {
                if (!Int32.TryParse(n, out vs[idx]))
                {
                    vs[idx] = -999; // mark for skip later
                }
                idx++;
            }
            idx = 0;
            foreach (string n in cmp)
            {
                if (!Int32.TryParse(n, out cs[idx]))
                {
                    cs[idx] = -999; // mark for skip later
                }
                idx++;
            }
            for (int i = 0; i < len; i++)
            {
                // skip non digits
                if ((vs[i] == -999) || (cs[i] == -999))
                {
                    continue;
                }
                if (vs[i] < cs[i])
                {
                    return (false);
                }
                else if (vs[i] > cs[i])
                {
                    return (true);
                }
            }
            return (true);
        }
    }
}
