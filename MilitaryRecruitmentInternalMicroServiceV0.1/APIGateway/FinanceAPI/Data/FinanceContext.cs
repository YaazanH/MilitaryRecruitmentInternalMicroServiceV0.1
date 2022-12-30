using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Data
{
    public class FinanceContext : DbContext
    {
        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        { }

        public DbSet<FinanceAPI.Models.Finance> FinanceContextDBS { get; set; }
        public DbSet<FinanceAPI.Models.OrderRef> OrderRefDBS { get; set; }
    }
}
