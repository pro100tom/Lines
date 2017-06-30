using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lines
{
    static class BalSizeEventManager
    {
        static Random random = new Random();

        public static async void Grow(this Ellipse ball, double desiredBallSize, double cellSize)
        {
            //ChangeSize(ball, 1, desiredSize);

            var timer = new DispatcherTimer();
            var step = 8;

            timer.Tick += (sender, args) =>
            {
                var width = ball.ActualWidth;
                if (width >= desiredBallSize - step)
                {
                    timer.Stop();

                    return;
                }

                width += step;
                ball.Width = width;
                ball.Height = width;

                var margin = (cellSize - width) / 2;
                ball.Margin = new Thickness(margin);
            };

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);

            int delay = random.Next(300);
            await Task.Delay(delay);
            timer.Start();
        }

        public static void Shrink(this Ellipse ball, double desiredSize)
        {
            ChangeSize(ball, ball.ActualWidth, 1);
        }

        private static void ChangeSize(Ellipse ball, double initialSize, double desiredSize)
        {
            
        }
    }
}
