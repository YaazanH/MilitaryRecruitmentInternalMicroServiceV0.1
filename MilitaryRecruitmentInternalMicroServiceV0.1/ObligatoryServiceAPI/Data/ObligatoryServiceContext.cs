using Microsoft.EntityFrameworkCore;

namespace ObligatoryServiceAPI.Data
{
    public class ObligatoryServiceContext : DbContext
    {
        public ObligatoryServiceContext(DbContextOptions<ObligatoryServiceContext> options) : base(options)
        { }

        public DbSet<ObligatoryServiceAPI.Models.ObligatoryService> ObligatoryServiceDB { get; set; }
    }
}