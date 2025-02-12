using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccelokaAPI.Models
{
    public class BookedTicket
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public ICollection<BookedTicketDetail> BookedTicketDetails { get; set; } = new List<BookedTicketDetail>();
    }
}
