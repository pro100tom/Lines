using System.Collections.Generic;
using System.Windows.Media;
using static Lines.Design;

namespace Lines
{
    static class Settings
    {
        public static int FieldSideCellCount { get { return 11; } }
        public static int FieldCellCount { get { return FieldSideCellCount * FieldSideCellCount; } }

        public static int SpawnBallQtyMin { get { return 3; } }
        public static int SpawnBallQtyMax { get { return 4; } }

        public static int MinNumberOfBallsRequired { get { return 5; } }

        public static List<Color> AllowedColours
        {
            get
            {
                return new List<Color>() {
                    BallColorYellow,
                    BallColorBlue,
                    BallColorAliceBlue,
                    BallColorGreen,
                    BallColorRed,
                    BallColorBlack,
                    BallColorPink,
                };
            }
        }
    }
}
