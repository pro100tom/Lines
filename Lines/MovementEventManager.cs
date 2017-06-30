using System;
using System.Windows.Threading;

namespace Lines
{
    static class MovementEventManager
    {
        public static event DefaultEventHandler NotifyMoveCompleted;
        public static event MovementEventHandler NotifyStepMade;

        public static void StartMovementTimer(double initialCoordinate, double step, double threshold, Direction direction)
        {
            var timer = new DispatcherTimer();

            var coordinate = initialCoordinate;

            timer.Tick += (sender, args) =>
            {
                if (direction == Direction.Left || direction == Direction.Up)
                {
                    step = -Math.Abs(step);
                }

                coordinate += step;

                if (Math.Abs(coordinate) >= threshold)
                {
                    timer.Stop();
                    NotifyMoveCompleted?.Invoke(null);
                    return;
                }

                NotifyStepMade?.Invoke(coordinate, direction);
            };

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            timer.Start();
        }
    }
}
