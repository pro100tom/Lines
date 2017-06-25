using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace Lines
{
    public delegate void DefaultEventHandler(object sender);

    static class Helper
    {
        static Random random = new Random();
        static DispatcherTimer timer = new DispatcherTimer();

        public static Color GetColor(string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);

            return color;
        }

        public static T FetchRandomItem<T>(IList<T> collection)
        {
            var result = random.Next(0, collection.Count());
            var element = collection.ElementAt(result);
            collection.RemoveAt(result);

            return element;
        }

        public static T Dequeue<T>(this List<T> list)
        {
            T first = default(T);

            if (list.Any())
            {
                first = list.FirstOrDefault();
                list.RemoveAt(0);
            }

            return first;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
