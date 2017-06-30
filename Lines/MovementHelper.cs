using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    static class MovementHelper
    {
        public static List<MovementInfo> GetMovementInfoList(List<int> indices)
        {
            var movementInfoList = new List<MovementInfo>();
            for (int i = 1; i < indices.Count; i++)
            {
                var firstIndex = indices[i - 1].GetIndex2D();
                var secondIndex = indices[i].GetIndex2D();

                var result = secondIndex.Subtract(firstIndex);

                Direction direction = Direction.Right;

                if (result.Item1 == -1)
                {
                    direction = Direction.Up;
                }
                else if (result.Item1 == 1)
                {
                    direction = Direction.Down;
                }
                else if (result.Item2 == -1)
                {
                    direction = Direction.Left;
                }

                if (movementInfoList.Any() && movementInfoList[movementInfoList.Count - 1].Direction == direction)
                {
                    var lastInfoItem = movementInfoList.Last();
                    lastInfoItem.StepNumber++;
                }
                else
                {
                    movementInfoList.Add(new MovementInfo() { Direction = direction, StepNumber = 1 });
                }
            }

            return movementInfoList;
        }

        public static int GetIndexByOffset(int currentIndex, Direction direction, int steps, int stepsInRow)
        {
            var currentIndex2D = currentIndex.GetIndex2D();

            var resultX = currentIndex2D.Item1;
            var resultY = currentIndex2D.Item2;

            if (direction == Direction.Left || direction == Direction.Up)
            {
                steps = -Math.Abs(steps);
            }

            if (direction == Direction.Left || direction == Direction.Right)
            {
                resultY = currentIndex2D.Item2 + steps;
            }
            else
            {
                resultX = currentIndex2D.Item1 + steps;
            }

            if (resultX < 0 || resultX > stepsInRow - 1 || resultY < 0 || resultY > stepsInRow - 1)
            {
                return -1;
            }

            var resultIndex2D = Tuple.Create(resultX, resultY);
            var resultIndex = resultIndex2D.GetIndex1D();

            return resultIndex;
        }
    }
}
