using Microsoft.EntityFrameworkCore;
namespace SchoolPostponementAPI.Data
{
    public class SchoolPostponementContext : DbContext
    {
        public SchoolPostponementContext(DbContextOptions<SchoolPostponementContext> options) : base(options)
        { }

        public DbSet<SchoolPostponementAPI.Models.SchoolPostponement> schoolDBS { get; set; }
    }
}