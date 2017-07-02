﻿using System.Windows.Media;
using static Lines.ExtensionMethods;

namespace Lines
{
    static class Design
    {
        public static Color CellColorIdle { get { return GetColor("#DDDDDD"); } }
        public static Color CellColorHover { get { return GetColor("#b0e0fb"); } }
        public static Color CellColorPressed { get { return GetColor("#a5daf7"); } }
        public static Color CellColorSelected { get { return GetColor("#b0e0fb"); } }
        public static Color CellColorPath { get { return GetColor("#D4EFFF"); } }
        public static Color CellColorGhost { get { return GetColor("#d9e9f2"); } }
        public static Color CellColorSelectedGhost { get { return GetColor("#c0d7e4"); } }
        public static Color CellColorInaccessible { get { return GetColor("#fbb0b0"); } }
        public static Color CellColorInaccessiblePressed { get { return GetColor("#faa6a6"); } }

        public static double CellBorderThinknessLength { get { return 0.5d; } }
        public static Color CellBorderColor { get { return Colors.Black; } }

        public static double BallSizePercentage { get { return 0.7d; } }

        public static int ZIndexDefault { get { return 1; } }
        public static int ZIndexTop { get { return 2; } }

        public static Color BallColorYellow { get { return GetColor("#f5da46"); } }
        public static Color BallColorGreen { get { return GetColor("#48e76a"); } }
        public static Color BallColorRed { get { return GetColor("#d72020"); } }
        public static Color BallColorBlue { get { return GetColor("#2795ce"); } }
        public static Color BallColorAliceBlue { get { return Colors.AliceBlue; } }
        public static Color BallColorBlack { get { return GetColor("#3b3b3b"); } }
        public static Color BallColorPink { get { return GetColor("#faa7fb"); } }

        public static Color GetColor(string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);

            return color;
        }
    }
}
