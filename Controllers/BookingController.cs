using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccelokaAPI.Models;
using AccelokaAPI.Data;

namespace AccelokaAPI.Controllers
{
    [Route("api/v1/book-ticket")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult BookTicket([FromBody] BookingRequest request)
        {
            if (request == null || request.Tickets == null || !request.Tickets.Any())
            {
                return BadRequest(new { message = "Invalid request body." });
            }

            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();
            var tickets = _context.Tickets.Include(t => t.Category)
                                          .Where(t => ticketCodes.Contains(t.Code))
                                          .ToList();
            
            if (tickets.Count != ticketCodes.Count)
            {
                return NotFound(new { message = "One or more ticket codes are invalid." });
            }

            var booking = new BookedTicket();
            var categorySummaries = new Dictionary<string, CategoryBookingSummary>();
            decimal totalBookingPrice = 0;

            foreach (var ticketRequest in request.Tickets)
            {
                var ticket = tickets.FirstOrDefault(t => t.Code == ticketRequest.TicketCode);
                if (ticket == null) continue;

                if (ticket.Quota < ticketRequest.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient quota for ticket {ticket.Code}." });
                }

                if (ticket.EventDate <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = $"Cannot book past event ticket {ticket.Code}." });
                }

                decimal totalPrice = ticket.Price * ticketRequest.Quantity;
                ticket.Quota -= ticketRequest.Quantity;
                
                booking.BookedTicketDetails.Add(new BookedTicketDetail
                {
                    TicketId = ticket.Id,
                    Quantity = ticketRequest.Quantity,
                    TotalPrice = totalPrice
                });

                if (!categorySummaries.ContainsKey(ticket.Category.CategoryName))
                {
                    categorySummaries[ticket.Category.CategoryName] = new CategoryBookingSummary
                    {
                        CategoryName = ticket.Category.CategoryName,
                        SummaryPrice = 0,
                        Tickets = new List<TicketBookingDetail>()
                    };
                }

                categorySummaries[ticket.Category.CategoryName].Tickets.Add(new TicketBookingDetail
                {
                    TicketCode = ticket.Code,
                    TicketName = ticket.Name,
                    Price = totalPrice
                });
                
                categorySummaries[ticket.Category.CategoryName].SummaryPrice += totalPrice;
                totalBookingPrice += totalPrice;
            }

            _context.BookedTickets.Add(booking);
            _context.SaveChanges();

            var response = new BookingResponse
            {
                PriceSummary = totalBookingPrice,
                TicketsPerCategories = categorySummaries.Values.ToList()
            };

            return Ok(response);
        }
    }
}
