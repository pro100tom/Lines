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

        public static double BallSizePercentage = 0.7d;

        public static int zIndexDefault = 1;
        public static int zIndexTop = 2;
    }
}
