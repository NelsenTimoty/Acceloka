using System.Collections.Generic;

namespace AccelokaAPI.Models
{
    public class BookingResponse
    {
        public decimal PriceSummary { get; set; }
        public List<CategoryBookingSummary> TicketsPerCategories { get; set; } = new();
    }

    public class CategoryBookingSummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal SummaryPrice { get; set; }
        public List<TicketBookingDetail> Tickets { get; set; } = new();
    }

    public class TicketBookingDetail
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid BookingId { get; set; }
    }
}
