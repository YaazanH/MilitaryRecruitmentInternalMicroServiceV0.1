using Microsoft.EntityFrameworkCore;

namespace AlonePostponement.Data
{
    public class AlonePostponementContext : DbContext
    {
        public AlonePostponementContext(DbContextOptions<AlonePostponementContext> options) : base(options)
        { }

        public DbSet<AlonePostponement.Models.AlonePostponement> AlonePostponementDBS { get; set; }
    }
}