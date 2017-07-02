using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Lines.GameHelper;

namespace Lines
{
    static class BalSizeEventManager
    {
        public static event DefaultEventHandler NotifyGrowFinished;
        public static event DefaultEventHandler NotifyShrinkFinished;

        public static async void Grow(this Ellipse ball, double desiredBallSize, double cellSize)
        {
            var timer = new DispatcherTimer();
            var step = 8;

            timer.Tick += (sender, args) =>
            {
                var width = ball.ActualWidth;
                if (width >= desiredBallSize - step)
                {
                    timer.Stop();
                    NotifyGrowFinished?.Invoke(ball);

                    return;
                }

                width += step;
                ball.Width = width;
                ball.Height = width;

                var margin = (cellSize - width) / 2;
                ball.Margin = new Thickness(margin);
            };

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);

            int delay = RandomObject.Next(300);
            await Task.Delay(delay);
            timer.Start();
        }

        public static void Shrink(this Ellipse ball, double desiredBallSize, double cellSize)
        {
            var timer = new DispatcherTimer();
            var step = 8;

            timer.Tick += (sender, args) =>
            {
                var width = ball.ActualWidth;
                if (width <= desiredBallSize + step)
                {
                    timer.Stop();
                    NotifyShrinkFinished?.Invoke(ball);

                    return;
                }

                width -= step;
                ball.Width = width;
                ball.Height = width;

                var margin = (cellSize - width) / 2;
                ball.Margin = new Thickness(margin);
            };

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            timer.Start();
        }
    }
}
