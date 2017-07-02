using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lines
{
    static class BallHelper
    {
        public static Color GetColor(this Ellipse ellipse)
        {
            var brush = ellipse.Fill as SolidColorBrush;

            return brush.Color;
        }
    }
}
