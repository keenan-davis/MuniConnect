using System.ComponentModel.DataAnnotations;

namespace MuniConnect.Models
{
    public class Issue
    {
        public int Id { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        public string AttachmentPath { get; set; } = string.Empty;
        public DateTime DateReported { get; set; } = DateTime.Now;

        public string UserFeedback { get; set; } = string.Empty;
        public int? SatisfactionRating { get; set; }
    }
}
