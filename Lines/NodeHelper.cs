using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lines.Settings;

namespace Lines
{
    static class NodeHelper
    {
        public static Tuple<int, int> GetIndex2D(this Node node)
        {
            int index = node.Index;
            int row = index / FieldSideCellCount;
            int column = index % FieldSideCellCount;
            var tuple = Tuple.Create(row, column);

            return tuple;
        }
    }
}
