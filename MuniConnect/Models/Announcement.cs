namespace MuniConnect.Models
{
    public class Announcement
    {
    
        
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public DateTime PublishDate { get; set; }
            public string Location { get; set; } = string.Empty;
        }
}



