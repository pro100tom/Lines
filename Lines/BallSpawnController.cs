using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lines
{
    static class BallSpawnController
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

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
            lock (syncLock)
            {
                var result = random.Next(min, max + 1);

                return result;
            }
        }
    }
}
