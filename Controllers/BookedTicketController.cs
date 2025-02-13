using Microsoft.AspNetCore.Mvc;
using AccelokaAPI.Repositories;
using AccelokaAPI.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace AccelokaAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly IBookedTicketRepository _bookedTicketRepository;

        public BookedTicketController(IBookedTicketRepository bookedTicketRepository)
        {
            _bookedTicketRepository = bookedTicketRepository;
        }

        [HttpGet("get-booked-ticket/bookedTicketId")]
        public async Task<IActionResult> GetBookedTicketDetails([FromQuery] List<Guid> bookedTicketIds)
        {
            Log.Information("Received GetBookedTicketDetails request: {@BookedTicketIds}", bookedTicketIds);

            if (bookedTicketIds == null || !bookedTicketIds.Any())
            {
                Log.Warning("GetBookedTicketDetails failed: No booked ticket IDs provided.");
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Bad Request",
                    Status = 400,
                    Detail = "Booked Ticket IDs are required.",
                    Instance = HttpContext.Request.Path
                });
            }

            var bookedTickets = await _bookedTicketRepository.GetBookedTicketDetails(bookedTicketIds);
            if (bookedTickets == null || !bookedTickets.Any())
            {
                 Log.Warning("GetBookedTicketDetails failed: No booked tickets found for IDs {@BookedTicketIds}", bookedTicketIds);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Not Found",
                    Status = 404,
                    Detail = "No booked tickets found for the given IDs.",
                    Instance = HttpContext.Request.Path
                });
            }

            var groupedTickets = bookedTickets
                .GroupBy(t => t.CategoryName)
                .Select(g => new
                {
                    qtyPerCategory = g.Sum(t => t.Quantity),
                    categoryName = g.Key,
                    tickets = g.Select(t => new
                    {
                        ticketCode = t.TicketCode,
                        ticketName = t.TicketName,
                        eventDate = t.EventDate.ToString("dd-MM-yyyy HH:mm")
                    }).ToList()
                });

            return Ok(groupedTickets);
        }

        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{qty}")]
        public async Task<IActionResult> RevokeBookedTicket(Guid bookedTicketId, string ticketCode, int qty)
        {
            Log.Information("Received RevokeBookedTicket request: BookedTicketId={BookedTicketId}, TicketCode={TicketCode}, Quantity={Qty}",
                            bookedTicketId, ticketCode, qty);
            // Validate if the booked ticket exists
            var bookedTicket = await _bookedTicketRepository.GetBookedTicketById(bookedTicketId);
            if (bookedTicket == null)
            {
                Log.Warning("RevokeBookedTicket failed: BookedTicketId {BookedTicketId} not found.", bookedTicketId);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Not Found",
                    Status = 404,
                    Detail = "Booked Ticket ID not found.",
                    Instance = HttpContext.Request.Path
                });
            }

            // Validate if the ticket code exists under this booked ticket
            var ticketDetail = bookedTicket.BookedTicketDetails.FirstOrDefault(t => t.Ticket.Code == ticketCode);
            if (ticketDetail == null)
            {
                Log.Warning("RevokeBookedTicket failed: TicketCode {TicketCode} not found for BookedTicketId {BookedTicketId}.",
                          ticketCode, bookedTicketId);
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Not Found",
                    Status = 404,
                    Detail = "Ticket Code not found for the given Booked Ticket ID.",
                    Instance = HttpContext.Request.Path
                });
            }

            // Validate quantity
            if (qty > ticketDetail.Quantity)
            {   
                Log.Warning("RevokeBookedTicket failed: Quantity {Qty} exceeds booked ticket quantity {AvailableQty} for TicketCode {TicketCode}.",
                          qty, ticketDetail.Quantity, ticketCode);
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Bad Request",
                    Status = 400,
                    Detail = "Quantity exceeds the booked ticket quantity.",
                    Instance = HttpContext.Request.Path
                });
            }

            // Deduct quantity or remove ticket
            ticketDetail.Quantity -= qty;
            ticketDetail.Ticket.Quota += qty;
            if (ticketDetail.Quantity == 0)
            {
                bookedTicket.BookedTicketDetails.Remove(ticketDetail);
            }

            // If no tickets left in booked ticket, remove the booked ticket entry
            if (!bookedTicket.BookedTicketDetails.Any())
            {
                await _bookedTicketRepository.DeleteBookedTicket(bookedTicketId);
            }
            else
            {
                await _bookedTicketRepository.UpdateBookedTicket(bookedTicket);
            }

            return Ok(new
            {
                ticketCode = ticketDetail.Ticket.Code,
                ticketName = ticketDetail.Ticket.Name,
                categoryName = ticketDetail.Ticket.Category.CategoryName,
                remainingQty = ticketDetail.Quantity
            });
        }
        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(Guid bookedTicketId, [FromBody] List<TicketUpdateModel> tickets)
        {   
            Log.Information("Received EditBookedTicket request: BookedTicketId={BookedTicketId}, Tickets={@Tickets}",
                            bookedTicketId, tickets);
            if (tickets == null || !tickets.Any())
            {
                Log.Warning("EditBookedTicket failed: No tickets provided.");
                return BadRequest(new ProblemDetails
                {   
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Status = 400,
                    Title = "Bad Request",
                    Detail = "Tickets are required.",
                    Instance = HttpContext.Request.Path
                });
            }

            var result = await _bookedTicketRepository.EditBookedTicket(bookedTicketId, tickets);
            if (!result)
            {   
                Log.Warning("EditBookedTicket failed: BookedTicketId {BookedTicketId} or TicketCodes {@TicketCodes} not found, or quantity exceeds quota.",
                          bookedTicketId, tickets.Select(t => t.KodeTicket).ToList());
                return NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Not Found",
                    Status = 404,
                    Detail = "Booked Ticket ID or Ticket Code not found, or quantity exceeds quota.",
                    Instance = HttpContext.Request.Path
                });
            }

            var bookedTicket = await _bookedTicketRepository.GetBookedTicketById(bookedTicketId);
            var updatedTickets = bookedTicket.BookedTicketDetails
                .Where(d => tickets.Any(t => t.KodeTicket == d.Ticket.Code))
                .Select(d => new
                {
                    ticketCode = d.Ticket.Code,
                    ticketName = d.Ticket.Name,
                    categoryName = d.Ticket.Category.CategoryName,
                    remainingQty = d.Quantity
                }).ToList();

            return Ok(updatedTickets);
        }
    }
}
