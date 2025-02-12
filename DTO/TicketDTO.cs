namespace AccelokaAPI.DTOs
{
    public class TicketDTO
    {
        public string Category { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int RemainingQuota { get; set; }
    }

}
