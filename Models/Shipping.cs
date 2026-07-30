namespace shipping_api.Models
{

    public class Shipment
    {
        public int Id { get; set; }
        public string TrackingId { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;  
        public string Destination { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}