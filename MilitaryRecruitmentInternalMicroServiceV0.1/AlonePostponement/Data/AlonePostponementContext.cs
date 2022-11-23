using Microsoft.EntityFrameworkCore;

namespace AlonePostponement.Data
{
    public class AlonePostponementContext : DbContext
    {
        public AlonePostponementContext(DbContextOptions<AlonePostponementContext> options) : base(options)
        { }

        public DbSet<AlonePostponement.Models.AlonePostponement> AlonePostponementDBS { get; set; }

        public DbSet<AlonePostponement.Models.RequestStatues> RequestStatuesDBS { get; set;}
        public DbSet<AlonePostponement.Models.DeadBrothers> DeadBrothersDBS { get; set;}
        public DbSet<AlonePostponement.Models.BrotherEill> BrotherEillDBS { get; set;}
        public DbSet<AlonePostponement.Models.BrothersID> BrothersIDDBS { get; set;}
        public DbSet<AlonePostponement.Models.HaveBrothers> HaveBrothersDBS { get; set;}


    }
}