namespace QManager.Models
{
    // Obiect de transfer de date (DTO) pentru stocarea proprietăților vizuale ale unui tichet
    public class TicketStateDto
    {
        public int TicketId { get; set; }
        public string TicketLabel { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "On hold" sau "Ready"
    }
}