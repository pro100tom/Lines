﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Lines.Physics;

namespace Lines
{
    static class BounceHelper
    {
        static Dictionary<Ellipse, LinesTimer> timerDictionary = new Dictionary<Ellipse, LinesTimer>();

        public static void BounceStart(this Ellipse ellipse)
        {
            foreach (var item in timerDictionary)
            {
                if (item.Value.IsEnabled)
                {
                    item.Value.ScheduleStop();
                }
            }

            if (!timerDictionary.ContainsKey(ellipse))
            {
                timerDictionary[ellipse] = new LinesTimer();
            }

            var timer = timerDictionary[ellipse];

            var margin = ellipse.Margin;
            var steps = CalculateObjectTravelSteps(margin.Top * 0.82d, 0, 70);
            var stepsBackup = new List<double>(steps);

            int direction = 1;

            if (!timer.ShallStop)
            {
                timer.Tick += (sender, args) =>
                {
                    if (!steps.Any())
                    {
                        steps = new List<double>(stepsBackup);
                        direction *= -1;

                        if (direction < 0)
                        {
                            steps.Reverse();
                        }
                        else
                        {
                            var timerLocal = sender as LinesTimer;

                            if (timerLocal.ShallStop)
                            {
                                timerLocal.Stop();
                            }
                        }
                    }

                    double step = steps.Dequeue();

                    margin.Top += step * direction;
                    margin.Bottom -= step * direction;

                    ellipse.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
                };
            }

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            timer.Start();
        }

        public static void BounceStop(this Ellipse ellipse)
        {
            if (!timerDictionary.Any() || !timerDictionary.ContainsKey(ellipse)) { return; }

            var timer = timerDictionary[ellipse];

            if (ellipse.IsBouncing())
            {
                timer.ScheduleStop();
            }
        }

        public static bool IsBouncing(this Ellipse ellipse)
        {
            if (!timerDictionary.Any()) { return false; }

            var timer = timerDictionary[ellipse];

            return timer.IsEnabled;
        }
    }
}
