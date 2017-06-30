using System.Collections.Generic;
using System.Linq;

namespace Lines
{
    /*class PathHistoryManager
    {
        //static PathHistoryManager instance = new PathHistoryManager();
        List<List<int>> pathHistory;

        private PathHistoryManager()
        {
            Pathfinder.Instance.NotifyPathFound += PathHistoryManager_NotifyPathFound;

            pathHistory = new List<List<int>>();
        }

        static PathHistoryManager()
        {

        }

        public void AddPathToHistory(List<int> path)
        {
            pathHistory.Add(path);

            if (pathHistory.Count > 50)
            {
                pathHistory.RemoveAt(0);
            }
        }

        public List<int> GetLatestPath()
        {
            var result = pathHistory.Any() ? pathHistory.Last() : null;

            return result;
        }

        public void GetLatestPathDirections()
        {

        }

        private void PathHistoryManager_NotifyPathFound(object sender)
        {
            var path = sender as List<int>;

            AddPathToHistory(path);
        }

        public static PathHistoryManager Instance
        {
            get
            {
                return instance;
            }
        }
    }*/
}
