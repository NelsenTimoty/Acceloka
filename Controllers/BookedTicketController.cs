using Microsoft.AspNetCore.Mvc;
using AccelokaAPI.Repositories;
using AccelokaAPI.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTicketDetails([FromQuery] List<Guid> bookedTicketIds)
        {
            if (bookedTicketIds == null || !bookedTicketIds.Any())
            {
                return BadRequest(new
                {
                    type = "https://example.com/probs/bad-request",
                    title = "Bad Request",
                    status = 400,
                    detail = "Booked Ticket IDs are required.",
                    instance = HttpContext.Request.Path
                });
            }

            var bookedTickets = await _bookedTicketRepository.GetBookedTicketDetails(bookedTicketIds);
            if (bookedTickets == null || !bookedTickets.Any())
            {
                return NotFound(new
                {
                    type = "https://example.com/probs/not-found",
                    title = "Not Found",
                    status = 404,
                    detail = "No booked tickets found for the given IDs.",
                    instance = HttpContext.Request.Path
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
    }
}
