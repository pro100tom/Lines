using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    static class Physics
    {
        public static List<double> CalculateObjectTravelSteps(double initialHeight, double endHeight, int timeStep)
        {
            var steps = new List<double>();

            double currentHeight = initialHeight;
            double currentTime = 0.0d;

            do
            {
                currentTime += Convert.ToDouble(timeStep) / 1000;

                double distanceStep = CalculateFreeFallDistance(currentTime);
                steps.Add(distanceStep);

                currentHeight -= distanceStep;
            } while (currentHeight > endHeight);

            return steps;
        }

        public static readonly double g = 20.0d;

        public static double CalculateFreeFallDistance(double time)
        {
            return g * Math.Pow(time, 2.0d) / 2;
        }
    }
}
