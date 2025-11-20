using MuniConnect.Models;

namespace MuniConnect.Data
{
    public class GraphServiceRequestRepository
    {
        private readonly Dictionary<string, List<string>> _adjacencyList = new();
        private readonly Dictionary<string, ServiceRequest> _requestMap = new();

        public GraphServiceRequestRepository(BSTServiceRequestRepository bstRepo)
        {
            // load all requests into graph nodes
            foreach (var req in bstRepo.GetAll())
            {
                AddRequest(req);
            }

            // OPTIONAL dependencies
            //AddDependency("REQ001", "REQ002");
            //AddDependency("REQ002", "REQ003");
        }

        public void AddRequest(ServiceRequest request)
        {
            if (!_requestMap.ContainsKey(request.RequestId))
                _requestMap[request.RequestId] = request;

            if (!_adjacencyList.ContainsKey(request.RequestId))
                _adjacencyList[request.RequestId] = new List<string>();
        }

        public void AddDependency(string fromId, string toId)
        {
            if (!_adjacencyList.ContainsKey(fromId))
                _adjacencyList[fromId] = new List<string>();

            _adjacencyList[fromId].Add(toId);
        }

        public List<ServiceRequest> BFS(string startId)
        {
            List<ServiceRequest> visitedList = new();
            HashSet<string> visited = new();
            Queue<string> queue = new();

            queue.Enqueue(startId);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();

                if (!visited.Contains(current))
                {
                    visited.Add(current);

                    if (_requestMap.TryGetValue(current, out var reqObj))
                        visitedList.Add(reqObj);

                    if (_adjacencyList.ContainsKey(current))
                    {
                        foreach (var neighbor in _adjacencyList[current])
                        {
                            if (!visited.Contains(neighbor))
                                queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return visitedList;
        }
    }
}
