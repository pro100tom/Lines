using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lines
{
    static class Helper
    {
        static Random random = new Random();

        public static Color GetColor(string hex)
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }

        public static int GetBallSpawnQty(int min, int max)
        {
            Random random = new Random();
            var result = random.Next(min, max + 1);

            return result;
        }

        public static T FetchRandomItem<T>(IList<T> collection)
        {
            var result = random.Next(0, collection.Count());
            var element = collection.ElementAt(result);
            collection.RemoveAt(result);

            return element;
        }
    }
}
