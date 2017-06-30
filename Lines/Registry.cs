using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    static class Registry
    {
        static Dictionary<string, object> items = new Dictionary<string, object>();

        public static void PutItem(string key, object item)
        {
            items[key] = item;
        }

        public static object GetItem(string key)
        {
            if (!items.ContainsKey(key))
            {
                return null;
            }

            return items[key];
        }

        public static bool DoesContainKey(string key)
        {
            return items.ContainsKey(key);
        }
    }
}
