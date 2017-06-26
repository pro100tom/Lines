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
        public static string CellPrefix = "cell";
        public static char CellNameSeparator = '_';
        public static Cell LastAccessedCell = null;

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

        public static List<int> GetCellNameIndices(IEnumerable<string> names)
        {
            var indices = new List<int>();

            foreach (var name in names)
            {
                int index = GetCellNameIndex(name);
                indices.Add(index);
            }

            return indices;
        }

        public static List<int> GetCellNeighbourIndices(int currentIndex)
        {
            var list = new List<int>();

            int left = currentIndex - 1;
            if (!(left < 0 || left % FieldSideCellCount == 0))
            {
                list.Add(left);
            }

            int right = currentIndex + 1;
            if (!(right >= FieldCellCount || currentIndex % FieldSideCellCount == 0))
            {
                list.Add(right);
            }

            int top = currentIndex - FieldSideCellCount;
            if (!(top < 1))
            {
                list.Add(top);
            }

            int bottom = currentIndex + FieldSideCellCount;
            if (!(bottom > FieldCellCount))
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
    }
}
