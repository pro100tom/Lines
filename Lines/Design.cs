using System.Windows.Media;
using static Lines.Helper;

namespace Lines
{
    static class Design
    {
        public static Color CellColorIdle = GetColor("#DDDDDD");
        public static Color CellColorHover = GetColor("#b0e0fb");
        public static Color CellColorPressed = GetColor("#a1d9f8");
        public static Color CellColorSelected = GetColor("#b0e0fb");
        public static Color CellColorPath = GetColor("#D4EFFF");
        public static Color CellColorGhost = GetColor("#d9e9f2");
        public static Color CellColorSelectedGhost = GetColor("#c0d7e4");
        public static Color CellColorInaccessible = GetColor("#fbb0b0");
        public static Color CellColorInaccessiblePressed = GetColor("#faa6a6");

        public static double CellBorderThinknessLength = 0.5d;
        public static Color CellBorderColor = Colors.Black;

        public static double BallSizePercentage = 0.7d;

        public static int zIndexDefault = 1;
        public static int zIndexTop = 2;
    }
}
