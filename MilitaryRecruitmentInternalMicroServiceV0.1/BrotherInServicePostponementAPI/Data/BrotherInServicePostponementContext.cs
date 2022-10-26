using Microsoft.EntityFrameworkCore;
namespace SchoolPostponementAPI.Data
{
    public class BrotherInServicePostponementContext : DbContext
    {
        public BrotherInServicePostponementContext(DbContextOptions<BrotherInServicePostponementContext> options) : base(options)
        { }

        public DbSet<BrotherInServicePostponementAPI.Models.BrotherInServicePostponement> BrotherInServicePostponementDBS { get; set; }
    }
}