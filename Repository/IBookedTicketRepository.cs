using AccelokaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccelokaAPI.Repositories
{
    public interface IBookedTicketRepository
    {
        Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds);
        Task<BookedTicket> GetBookedTicketById(Guid bookedTicketId);
        Task UpdateBookedTicket(BookedTicket bookedTicket);
        Task DeleteBookedTicket(Guid bookedTicketId);
        Task<int?> GetRemainingTicketQuota(string ticketCode);
        Task<bool> EditBookedTicket(Guid bookedTicketId, List<TicketUpdateModel> tickets);
    }
}