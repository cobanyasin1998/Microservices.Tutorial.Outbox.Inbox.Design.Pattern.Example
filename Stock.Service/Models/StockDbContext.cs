using Microsoft.EntityFrameworkCore;

namespace Stock.Service.Models
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderInbox> OrderInboxes { get; set; }
    }
}
