using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Lines.Settings;

namespace Lines
{
    static class CellHelper
    {
        public static int GetCellNameIndex(string cellName)
        {
            int index = 0;

            var parts = cellName.Split(CellNameSeparator);
            if (parts.Any())
            {
                index = Convert.ToInt32(parts.Last());
            }

            return index;
        }

        public static List<int> GetCellNeighbourIndices(int currentIndex)
        {
            var list = new List<int>();
            int cellCount = Convert.ToInt32(Math.Pow(FieldSideCellCount, 2));

            int left = currentIndex - 1;
            if (!(left < 1 || left % FieldSideCellCount == 0))
            {
                list.Add(left);
            }

            int right = currentIndex + 1;
            if (!(right > cellCount || currentIndex % FieldSideCellCount == 0))
            {
                list.Add(right);
            }

            int top = currentIndex - FieldSideCellCount;
            if (!(top < 1))
            {
                list.Add(top);
            }

            int bottom = currentIndex + FieldSideCellCount;
            if (!(bottom > cellCount))
            {
                list.Add(bottom);
            }

            list.Sort();

            return list;
        }

        public static string ConvertIndexToName(int index)
        {
            return String.Join(CellNameSeparator.ToString(), CellPrefix, index.ToString());
        }

        public static List<string> ConvertIndicesToNames(List<int> indices)
        {
            var names = new List<string>();

            foreach (int index in indices)
            {
                var name = ConvertIndexToName(index);
                names.Add(name);
            }

            return names;
        }

        public static char CellNameSeparator
        {
            get
            {
                return '_';
            }
        }
    }
}
