using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccelokaAPI.Models;
using AccelokaAPI.Data;
using Serilog;

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
        public async Task<IActionResult> BookTicket([FromBody] BookingRequest request)
        {
            Log.Information("Received booking request: {@Request}", request);
            
            if (request == null || request.Tickets == null || !request.Tickets.Any())
            {
                Log.Warning("Invalid booking request received.");
                return BadRequest(new ProblemDetails
                {   
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = 400,
                    Title = "Invalid Request",
                    Detail = "Request body is missing or invalid.",
                    Instance = HttpContext.Request.Path
                });
            }

            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();
            var tickets = await _context.Tickets.Include(t => t.Category)
                                                .Where(t => ticketCodes.Contains(t.Code))
                                                .ToListAsync();
            
            if (tickets.Count != ticketCodes.Count)
            {
                Log.Warning("One or more ticket codes are invalid. Requested: {@TicketCodes}", ticketCodes);
                return NotFound(new ProblemDetails
                {   
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = 404,
                    Title = "Tickets Not Found",
                    Detail = "One or more ticket codes are invalid.",
                    Instance = HttpContext.Request.Path
                });
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
                    Log.Warning("Insufficient quota for ticket {TicketCode}. Requested: {Qty}, Available: {Quota}",
                                ticket.Code, ticketRequest.Quantity, ticket.Quota);
                    return BadRequest(new ProblemDetails
                    {   
                        Type = "https://tools.ietf.org/html/rfc7807",
                        Status = 400,
                        Title = "Insufficient Quota",
                        Detail = $"Insufficient quota for ticket {ticket.Code}.",
                        Instance = HttpContext.Request.Path
                    });
                }

                if (ticket.EventDate <= DateTime.UtcNow)
                {   
                    Log.Warning("Attempt to book a past event ticket: {TicketCode}", ticket.Code);
                     return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Invalid Booking Date",
                        Detail = $"Cannot book past event ticket {ticket.Code}.",
                        Instance = HttpContext.Request.Path
                    });
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
                    Price = totalPrice,
                });
                
                categorySummaries[ticket.Category.CategoryName].SummaryPrice += totalPrice;
                totalBookingPrice += totalPrice;
            }

            _context.BookedTickets.Add(booking);
            await _context.SaveChangesAsync();

            Log.Information("Booking successful. Total price: {TotalPrice}, Booking ID: {BookingId}",
                            totalBookingPrice, booking.Id);

            var response = new BookingResponse
            {
                PriceSummary = totalBookingPrice,
                TicketsPerCategories = categorySummaries.Values.ToList()
            };

            return Ok(response);
        }
    }
}
