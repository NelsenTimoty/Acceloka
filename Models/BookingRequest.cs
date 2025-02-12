using System.Collections.Generic;

namespace AccelokaAPI.Models
{
    public class BookingRequest
    {
        public List<TicketBookingRequest> Tickets { get; set; } = new();
    }

    public class TicketBookingRequest
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
