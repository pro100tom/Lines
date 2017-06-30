using System;
using System.Collections.Generic;
using System.Linq;
using static Lines.Settings;

namespace Lines
{
    static class Pathfinder
    {
        static List<Node> nodes;
        static int rowCount = FieldSideCellCount;
        static List<Node> closeList, openList;
        static int movementCost = 1;

        public static void ResetLists()
        {
            closeList = new List<Node>();
            openList = new List<Node>();
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

            var currentNode = FindNode(targetPointIndex);
            if (currentNode.Parent == null) { return indices; }

            while (currentNode.Index != startPointIndex)
            {
                indices.Add(currentNode.Index);
                currentNode = currentNode.Parent;
            }

            indices.Add(startPointIndex);
            indices.Reverse();

            return indices;
        }

        private static List<Node> PerformSearch(int startPointIndex, int targetPointIndex)
        {
            var currentNode = FindNode(startPointIndex);
            var targetNode = FindNode(targetPointIndex);

            int heuristic = CalculateHeuristicValue(currentNode.Index.GetIndex2D(), targetNode.Index.GetIndex2D());
            currentNode.Heuristic = heuristic;

            closeList.Add(currentNode);
            closeList = closeList.Distinct().ToList();

            var neighbours = GetNeighbours(currentNode);
            neighbours = neighbours.Where(n => !closeList.Select(i => i.Index).Contains(n.Index)).ToList();
            openList.AddRange(neighbours);
            openList = openList.Except(closeList).Distinct().ToList();

            if (!openList.Any()) { return null; }

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

                heuristic = CalculateHeuristicValue(neighbour.Index.GetIndex2D(), targetNode.Index.GetIndex2D());
                neighbour.Heuristic = heuristic;
                neighbour.Movement = neighbour.Parent.Movement + movementCost;
            }

            var cheapest = GetCheapest(openList);

            return PerformSearch(cheapest.Index, targetNode.Index);
        }

        public static Node GetCheapest(List<Node> nodes)
        {
            var cheapestF = nodes.Min(n => n.F);
            var cheapestNodes = (from n in nodes
                                 where n.F == cheapestF
                                 select n).ToList();

            cheapestNodes = cheapestNodes.OrderBy(n => n.Index).ToList();
            var cheapest = cheapestNodes.First();

            return cheapest;
        }

        private static Node FindNode(int index)
        {
            return nodes.Where(n => n.Index == index).SingleOrDefault();
        }

        public static int CalculateHeuristicValue(Tuple<int, int> startPoint, Tuple<int, int> targetPoint)
        {
            int x = Math.Abs(targetPoint.Item1 - startPoint.Item1);
            int y = Math.Abs(targetPoint.Item2 - startPoint.Item2);
            int heuristic = x + y;

            return heuristic;
        }

        private static List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            var index2D = node.Index.GetIndex2D();

            if (index2D.Item1 > 0)
            {
                var topIndex2D = Tuple.Create(index2D.Item1 - 1, index2D.Item2);
                int index = topIndex2D.GetIndex1D();
                var topNode = FindNode(index);

                neighbours.Add(topNode);
            }

            if (index2D.Item2 > 0)
            {
                var leftIndex2D = Tuple.Create(index2D.Item1, index2D.Item2 - 1);
                int index = leftIndex2D.GetIndex1D();
                var leftNode = FindNode(index);

                neighbours.Add(leftNode);
            }

            if (index2D.Item2 < rowCount - 1)
            {
                var rightIndex2D = Tuple.Create(index2D.Item1, index2D.Item2 + 1);
                int index = rightIndex2D.GetIndex1D();
                var rightNode = FindNode(index);

                neighbours.Add(rightNode);
            }

            if (index2D.Item1 < rowCount - 1)
            {
                var bottomIndex2D = Tuple.Create(index2D.Item1 + 1, index2D.Item2);
                int index = bottomIndex2D.GetIndex1D();
                var bottomNode = FindNode(index);

                neighbours.Add(bottomNode);
            }

            neighbours.RemoveAll(n => n == null);
            neighbours = neighbours.OrderBy(n => n.Index).ToList();

            return neighbours;
        }
    }
}
