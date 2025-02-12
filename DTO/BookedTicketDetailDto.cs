using System;

namespace AccelokaAPI.Models
{
    public class BookedTicketDetailDto
    {
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public DateTime EventDate { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; }
    }
}
