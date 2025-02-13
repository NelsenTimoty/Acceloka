using Microsoft.AspNetCore.Mvc;
using AccelokaAPI.Repositories;
using AccelokaAPI.DTOs;
using Serilog;

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
            if (page < 1)
            {
                Log.Warning("Invalid page number: {Page}", page);
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Bad Request",
                    Status = 400,
                    Detail = "Page must be greater than 0.",
                    Instance = HttpContext.Request.Path
                });
            }

            Log.Information("Fetching available tickets with filters: " +
                            "Category={Category}, Code={Code}, Name={Name}, MaxPrice={MaxPrice}, " +
                            "MinEventDate={MinEventDate}, MaxEventDate={MaxEventDate}, " +
                            "OrderBy={OrderBy}, OrderState={OrderState}, Page={Page}",
                            category, code, name, maxPrice, minEventDate, maxEventDate, orderBy, orderState, page);

            try
            {
                var result = await _ticketRepository.GetAvailableTickets(
                    category, code, name, maxPrice, minEventDate, maxEventDate, 
                    orderBy ?? "Code",
                    orderState ?? "asc",
                    page);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching available tickets.");

                return StatusCode(500, new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Internal Server Error",
                    Status = 500,
                    Detail = "An error occurred while processing your request.",
                    Instance = HttpContext.Request.Path
                });
            }
        }
    }
}
