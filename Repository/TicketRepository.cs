using AccelokaAPI.Data;
using AccelokaAPI.Models;
using AccelokaAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AccelokaAPI.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10; // Pagination size

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<TicketDTO>> GetAvailableTickets(
            string? category, string? code, string? name, decimal? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate,
            string orderBy, string orderState, int page)
        {
            var query = _context.Tickets
                .Include(t => t.Category) // 🔹 Ensure Category is loaded
                .Where(t => t.Quota > 0)  // Only available tickets
                .AsQueryable();
            
            // 🔍 Filtering
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category != null && t.Category.CategoryName.Contains(category));
            }

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(t => t.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(t => t.Name.Contains(name));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxPrice.Value);
            }

            if (minEventDate.HasValue)
            {
                query = query.Where(t => t.EventDate >= minEventDate.Value);
            }

            if (maxEventDate.HasValue)
            {
                query = query.Where(t => t.EventDate <= maxEventDate.Value);
            }

            // 🔃 Sorting (Default: Order by `Code` ascending)
            bool isDescending = orderState.Equals("desc", StringComparison.OrdinalIgnoreCase);
            query = orderBy switch
            {
                "Category" => isDescending ? query.OrderByDescending(t => t.Category.CategoryName) : query.OrderBy(t => t.Category.CategoryName),
                "Code" => isDescending ? query.OrderByDescending(t => t.Code) : query.OrderBy(t => t.Code),
                "Name" => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "Price" => isDescending ? query.OrderByDescending(t => t.Price) : query.OrderBy(t => t.Price),
                "EventDate" => isDescending ? query.OrderByDescending(t => t.EventDate) : query.OrderBy(t => t.EventDate),
                _ => query.OrderBy(t => t.Code) // Default order
            };

            // 📄 Pagination
            int totalItems = await query.CountAsync();
            var tickets = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            // 🛠️ Transform to DTO
            var result = tickets.Select(t => new TicketDTO
            {
                Category = t.Category?.CategoryName ?? "Unknown", // ✅ This will now work
                Code = t.Code,
                Name = t.Name,
                EventDate = t.EventDate,
                Price = t.Price,
                RemainingQuota = t.Quota
            }).ToList();

            return new PaginatedResponse<TicketDTO>
            {
                Items = result,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize),
                CurrentPage = page
            };
        }

    }
}
