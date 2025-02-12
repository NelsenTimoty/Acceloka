using Microsoft.AspNetCore.Mvc;
using AccelokaAPI.Repositories;
using AccelokaAPI.DTOs;

namespace AccelokaAPI.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets(
            [FromQuery] string? category,
            [FromQuery] string? code,
            [FromQuery] string? name,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? minEventDate,
            [FromQuery] DateTime? maxEventDate,
            [FromQuery] string? orderBy = "Code",
            [FromQuery] string? orderState = "asc",
            [FromQuery] int page = 1)
        {
            if (page < 1) return BadRequest(new { message = "Page must be greater than 0" });

            try
            {
                var result = await _ticketRepository.GetAvailableTickets(
                    category, code, name, maxPrice, minEventDate, maxEventDate, 
                    orderBy ?? "Code", // ✅ Default: Order by `Code`
                    orderState ?? "asc", // ✅ Default: Ascending order
                    page);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = "Internal Server Error",
                    status = 500,
                    detail = ex.Message
                });
            }
        }
    }
}
