using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lines.Settings;

namespace Lines
{
    static class PathfindingHelper
    {
        static List<Node> nodes;
        static int rowCount;

        static HashSet<Node> closeList, openList;

        private static int movementCost = 10;

        static PathfindingHelper()
        {
            ResetLists();

            rowCount = FieldSideCellCount;
        }

        public static void ResetLists()
        {
            closeList = new HashSet<Node>();
            openList = new HashSet<Node>();
            nodes = new List<Node>();
        }

        public static List<int> FindPath(int startPointIndex, int targetPointIndex, List<int> cellIndices)
        {
            ResetLists();

            foreach (int index in cellIndices)
            {
                var node = new Node();
                node.Index = index;
                nodes.Add(node);
            }

            var indices = new List<int>();
            PerformSearch(startPointIndex, targetPointIndex);

            var targetNode = FindNode(targetPointIndex);
            var currentNode = targetNode;

            while (currentNode.Index != startPointIndex)
            {
                indices.Add(currentNode.Index);
                currentNode = currentNode.Parent;
            }

            indices.Add(startPointIndex);

            return indices;
        }

        private static List<Node> PerformSearch(int startPointIndex, int targetPointIndex)
        {
            var currentNode = FindNode(startPointIndex);
            var targetNode = FindNode(targetPointIndex);

            int heuristic = CalculateHeuristicValue(currentNode.GetIndex2D(), targetNode.GetIndex2D());
            currentNode.Heuristic = heuristic;

            closeList.Add(currentNode);

            var neighbours = GetNeighbours(currentNode);
            neighbours = neighbours.Where(n => !closeList.Select(i => i.Index).Contains(n.Index)).ToList();
            openList.UnionWith(neighbours);

            if (openList.Contains(targetNode))
            {
                targetNode.Parent = currentNode;

                return nodes;
            }

            foreach (var neighbour in neighbours)
            {
                if (neighbour.Parent == null || currentNode.Movement + movementCost < neighbour.Movement)
                {
                    neighbour.Parent = currentNode;
                }
                
                heuristic = CalculateHeuristicValue(neighbour.GetIndex2D(), targetNode.GetIndex2D());
                neighbour.Heuristic = heuristic;
                neighbour.Movement = neighbour.Parent.Movement + movementCost;
            }

            var closest = neighbours.Aggregate((one, two) => one.Heuristic < two.Heuristic ? one : two);

            return PerformSearch(closest.Index, targetNode.Index);
        }

        public static Node FindNode(int index)
        {
            return nodes.Where(n => n.Index == index).SingleOrDefault();
        }

        public static Tuple<int, int> ConvertTo2DIndex(int index)
        {
            int row = index / rowCount;
            int column = index % rowCount;
            var tuple = Tuple.Create(row, column);

            return tuple;
        }

        public static int ConvertTo1DIndex(Tuple<int, int> index2D)
        {
            int whole = index2D.Item1 * rowCount;
            int remainer = index2D.Item2;
            int index = whole + remainer;

            return index;
        }

        public static int CalculateHeuristicValue(Tuple<int, int> startPoint, Tuple<int, int> targetPoint)
        {
            int x = Math.Abs(targetPoint.Item1 - startPoint.Item1);
            int y = Math.Abs(targetPoint.Item2 - startPoint.Item2);
            int heuristic = x + y;

            return heuristic;
        }

        public static List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            var index2D = node.GetIndex2D();

            if (index2D.Item1 > 0)
            {
                var topIndex2D = Tuple.Create(index2D.Item1 - 1, index2D.Item2);
                int index = ConvertTo1DIndex(topIndex2D);
                var topNode = FindNode(index);

                neighbours.Add(topNode);
            }

            if (index2D.Item1 < rowCount - 1)
            {
                var bottomIndex2D = Tuple.Create(index2D.Item1 + 1, index2D.Item2);
                int index = ConvertTo1DIndex(bottomIndex2D);
                var bottomNode = FindNode(index);

                neighbours.Add(bottomNode);
            }

            if (index2D.Item2 > 0)
            {
                var leftIndex2D = Tuple.Create(index2D.Item1, index2D.Item2 - 1);
                int index = ConvertTo1DIndex(leftIndex2D);
                var leftNode = FindNode(index);

                neighbours.Add(leftNode);
            }

            if (index2D.Item2 < rowCount - 1)
            {
                var rightIndex2D = Tuple.Create(index2D.Item1, index2D.Item2 + 1);
                int index = ConvertTo1DIndex(rightIndex2D);
                var rightNode = FindNode(index);

                neighbours.Add(rightNode);
            }

            neighbours.RemoveAll(n => n == null);
            //neighbours = neighbours.OrderBy(n => n.Index).ToList();

            return neighbours;
        }
    }
}
