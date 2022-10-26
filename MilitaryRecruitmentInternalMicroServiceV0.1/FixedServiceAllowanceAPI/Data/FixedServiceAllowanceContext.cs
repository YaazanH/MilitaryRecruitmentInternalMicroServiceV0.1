using Microsoft.EntityFrameworkCore;

namespace FixedServiceAllowanceAPI.Data
{
    public class FixedServiceAllowanceContext : DbContext
    {
        public FixedServiceAllowanceContext(DbContextOptions<FixedServiceAllowanceContext> options) : base(options)
        { }

        public DbSet<FixedServiceAllowanceAPI.Models.FixedServiceAllowance> FixedServiceAllowanceContextDBS { get; set; }
    }
}