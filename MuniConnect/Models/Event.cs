namespace MuniConnect.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public string Location { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int Likes { get; set; } = 0;           
        public int Views { get; set; } = 0;          
        public double AverageRating { get; set; } = 0; 
        public int RatingCount { get; set; } = 0;

        public double EngagementScore => (Likes * 2) + (Views * 0.5) + (AverageRating * RatingCount);


    }
}
