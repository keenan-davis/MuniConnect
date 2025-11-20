namespace MuniConnect.Models
{
    public class ServiceRequest
    {
        public string RequestId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
    }
}
