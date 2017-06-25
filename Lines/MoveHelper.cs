﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Lines.BounceHelper;

namespace Lines
{
    enum Direction { Left, Up, Right, Down };

    static class MoveHelper
    {
        public static event DefaultEventHandler NotifyMoveStopped;
        static Dictionary<Ellipse, DispatcherTimer> timerDictionary = new Dictionary<Ellipse, DispatcherTimer>();
        static bool readyToMove;
        static List<int> indices;

        public static void ScheduleMove(List<int> indices)
        {
            readyToMove = true;
        }

        public static void Move(this Ellipse ellipse, Direction direction, double distance)
        {
            if (!readyToMove)
            {
                return;
            }

            readyToMove = false;

            timerDictionary[ellipse] = new DispatcherTimer();

            var timer = timerDictionary[ellipse];
            var margin = ellipse.Margin;
            int step = 20;

            timer.Tick += (sender, args) =>
            { 
                if (margin.Left >= distance - step)
                {
                    NotifyMoveStopped(ellipse);
                    timer.Stop();

                    return;
                }

                margin.Left += step;
                margin.Right += step;

                ellipse.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
            };

            timer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            timer.Start();
        }
    }
}
