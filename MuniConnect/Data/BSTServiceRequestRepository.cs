using MuniConnect.Models;

namespace MuniConnect.Data
{
    public class BSTServiceRequestRepository
    {
        private BSTNode? root;

        public BSTServiceRequestRepository()
        {
            SeedRequests();
        }

        public void Insert(ServiceRequest request)
        {
            if (request == null) return;
            root = InsertRecursive(root, request);
        }

        private BSTNode InsertRecursive(BSTNode? node, ServiceRequest request)
        {
            if (node == null)
                return new BSTNode(request);

            int compare = string.Compare(request.RequestId, node.Data.RequestId, StringComparison.OrdinalIgnoreCase);
            if (compare < 0)
                node.Left = InsertRecursive(node.Left, request);
            else if (compare > 0)
                node.Right = InsertRecursive(node.Right, request);
            else
                node.Data = request; // replace

            return node;
        }

        public List<ServiceRequest> GetAll()
        {
            var list = new List<ServiceRequest>();
            InOrderTraversal(root, list);
            return list;
        }

        private void InOrderTraversal(BSTNode? node, List<ServiceRequest> list)
        {
            if (node == null) return;
            InOrderTraversal(node.Left, list);
            list.Add(node.Data);
            InOrderTraversal(node.Right, list);
        }

        public ServiceRequest? FindById(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId)) return null;
            return SearchRec(root, requestId.Trim());
        }

        private ServiceRequest? SearchRec(BSTNode? node, string requestId)
        {
            if (node == null) return null;

            int compare = string.Compare(requestId, node.Data.RequestId, StringComparison.OrdinalIgnoreCase);

            if (compare == 0) return node.Data;
            if (compare < 0) return SearchRec(node.Left, requestId);
            return SearchRec(node.Right, requestId);
        }

        private void SeedRequests()
        {
            if (GetAll().Any()) return;

            var seeded = new List<ServiceRequest>
            {
                new ServiceRequest { RequestId = "REQ001", Title = "Water Leak", Description = "Pipe burst", Department = "Water", Status = "Pending", DateSubmitted = DateTime.UtcNow.AddDays(-2)},
                new ServiceRequest { RequestId = "REQ002", Title = "Power Outage", Description = "Transformer issue", Department = "Electricity", Status = "In Progress", DateSubmitted = DateTime.UtcNow.AddDays(-1)},
                new ServiceRequest { RequestId = "REQ003", Title = "Pothole Repair", Description = "Large pothole near city hall", Department = "Transport", Status = "Completed", DateSubmitted = DateTime.UtcNow.AddDays(-7)},
            };

            foreach (var r in seeded)
                Insert(r);
        }
    }
}
