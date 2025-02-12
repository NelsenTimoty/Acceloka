using AccelokaAPI.Data;
using AccelokaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccelokaAPI.Repositories
{
    public class BookedTicketRepository : IBookedTicketRepository
        {
            private readonly ApplicationDbContext _context;

            public BookedTicketRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<BookedTicketDetailDto>> GetBookedTicketDetails(List<Guid> bookedTicketIds)
            {
                return await _context.BookedTickets
                    .Where(bt => bookedTicketIds.Contains(bt.Id))  // Accept multiple GUIDs
                    .SelectMany(bt => bt.BookedTicketDetails)
                    .Select(d => new BookedTicketDetailDto
                    {
                        TicketCode = d.Ticket.Code,
                        TicketName = d.Ticket.Name,
                        EventDate = d.Ticket.EventDate,
                        Quantity = d.Quantity,
                        CategoryName = d.Ticket.Category.CategoryName
                    })
                    .ToListAsync();
            }
        }

}
