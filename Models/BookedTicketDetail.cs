using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccelokaAPI.Models
{
    public class BookedTicketDetail
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid TicketId { get; set; }
        
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        [ForeignKey("BookedTicketId")]
        public BookedTicket BookedTicket { get; set; }

        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public Guid BookedTicketId {get; set; }
    }
}
