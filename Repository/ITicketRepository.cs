using AccelokaAPI.Models;
using AccelokaAPI.DTOs;

namespace AccelokaAPI.Repositories
{
    public interface ITicketRepository
    {
        Task<PaginatedResponse<TicketDTO>> GetAvailableTickets(
            string? category, string? code, string? name, decimal? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate,
            string orderBy, string orderState, int page);
    }
}
