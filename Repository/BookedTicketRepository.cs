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
            public async Task<BookedTicket> GetBookedTicketById(Guid bookedTicketId)
            {
                return await _context.BookedTickets
                    .Include(bt => bt.BookedTicketDetails)
                    .ThenInclude(d => d.Ticket)
                    .ThenInclude(t => t.Category)
                    .FirstOrDefaultAsync(bt => bt.Id == bookedTicketId);
            }

            public async Task UpdateBookedTicket(BookedTicket bookedTicket)
            {
                _context.BookedTickets.Update(bookedTicket);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteBookedTicket(Guid bookedTicketId)
            {
                var bookedTicket = await _context.BookedTickets.FindAsync(bookedTicketId);
                if (bookedTicket != null)
                {
                    _context.BookedTickets.Remove(bookedTicket);
                    await _context.SaveChangesAsync();
                }
            }
            public async Task<int?> GetRemainingTicketQuota(string ticketCode)
            {
                return await _context.Tickets
                    .Where(t => t.Code == ticketCode)
                    .Select(t => (int?)t.Quota)
                    .FirstOrDefaultAsync() ?? 0;
            }
            public async Task<bool> EditBookedTicket(Guid bookedTicketId, List<TicketUpdateModel> tickets)
            {
                var bookedTicket = await _context.BookedTickets
                    .Include(bt => bt.BookedTicketDetails)
                    .ThenInclude(btd => btd.Ticket) // Ensure Ticket is loaded
                    .FirstOrDefaultAsync(bt => bt.Id == bookedTicketId);

                foreach (var item in tickets)
                {
                    var bookedDetail = bookedTicket.BookedTicketDetails
                        .FirstOrDefault(d => d.Ticket != null && d.Ticket.Code == item.KodeTicket);

                    if (bookedDetail != null)
                    {
                        int oldQuantity = bookedDetail.Quantity;  // Previous quantity
                        int newQuantity = item.Quantity;  // New quantity from request
                        int difference = newQuantity - oldQuantity;

                        bookedDetail.Quantity = newQuantity; // Update booked quantity

                        // Update ticket quota
                        var ticket = _context.Tickets.FirstOrDefault(t => t.Code == item.KodeTicket);
                        if (ticket != null)
                        {
                            ticket.Quota -= difference; // Adjust quota based on change
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }

}
