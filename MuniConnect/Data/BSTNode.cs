using MuniConnect.Models;

namespace MuniConnect.Data
{
    public class BSTNode
    {
        public ServiceRequest Data { get; set; }
        public BSTNode? Left { get; set; }
        public BSTNode? Right { get; set; }
        public BSTNode(ServiceRequest data)
        {
            Data = data;
        }
    }
}
