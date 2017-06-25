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
        public static Ellipse CreateBall()
        {
            var ball = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = false,
            };

            return ball;
        }

        public static int GetBallSpawnQty(int min, int max)
        {
            Random random = new Random();
            var result = random.Next(min, max + 1);

            return result;
        }
    }
}
