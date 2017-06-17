using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Lines.Helper;

namespace Lines
{
    static class Design
    {
        public static Color CellColorDefault = GetColor("#DDDDDD");
        public static Color CellColorHover = GetColor("#BEE6FD");
        public static Color CellColorClick = GetColor("#C4E5F6");

        public static double CellBorderThinknessLength = 0.5d;
        public static Color CellBorderColor = Colors.Black;
        public static bool CellBorderSnapToDevicePixelsFlag = true;

        public static bool UseBorder = true;
    }
}
