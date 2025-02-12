using AccelokaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccelokaAPI.Repositories
{
    public interface IBookedTicketRepository
    {
        Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds);
    }

}
