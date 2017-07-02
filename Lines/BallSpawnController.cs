using System;
using System.Windows.Media;
using System.Windows.Shapes;
using static Lines.Settings;
using static Lines.GameHelper;

namespace Lines
{
    static class BallSpawnController
    {
        public static Ellipse CreateBall()
        {
            var ball = new Ellipse()
            {
                //Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = false,
            };

            return ball;
        }

        public static int GetBallSpawnQty(int min, int max)
        {
            lock (SyncLock)
            {
                var result = RandomObject.Next(min, max + 1);

                return result;
            }
        }
    }
}
