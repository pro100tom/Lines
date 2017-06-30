using System;
using System.Collections.Generic;
using System.Linq;
using static Lines.Settings;

namespace Lines
{
    static class ExtensionMethods
    {
        static Random random = new Random();

        public static T FetchRandomItem<T>(this IList<T> collection)
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

        public static Tuple<int, int> GetIndex2D(this int index)
        {
            int row = index / FieldSideCellCount;
            int column = index % FieldSideCellCount;
            var tuple = Tuple.Create(row, column);

            return tuple;
        }

        public static int GetIndex1D(this Tuple<int, int> index2D)
        {
            int whole = index2D.Item1 * FieldSideCellCount;
            int remainer = index2D.Item2;
            int index = whole + remainer;

            return index;
        }

        public static Tuple<int, int> Subtract(this Tuple<int, int> targetIndex2D, Tuple<int, int> startIndex2D)
        {
            var result = new Tuple<int, int>(targetIndex2D.Item1 - startIndex2D.Item1, targetIndex2D.Item2 - startIndex2D.Item2);

            return result;
        }
    }
}
