namespace MuniConnect.Models
{
    public class TrackViewModel
    {
        public ServiceRequest Request { get; set; } = null!;
        public string ProgressClass { get; set; } = "";
        public string ProgressHex { get; set; } = "";
    }
}
