using Microsoft.EntityFrameworkCore;
namespace SchoolPostponementAPI.Data
{
    public class SchoolPostponementContext : DbContext
    {
        public SchoolPostponementContext(DbContextOptions<SchoolPostponementContext> options) : base(options)
        { }

        public DbSet<SchoolPostponementAPI.Models.SchoolPostponement> schoolDBS { get; set; }

        public DbSet<SchoolPostponementAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<SchoolPostponementAPI.Models.AsyncAge> AsyncAgeDBS { get; set; }

        public DbSet<SchoolPostponementAPI.Models.AsyncDroppedOut> AsyncDroppedOutDBS { get; set; }

        public DbSet<SchoolPostponementAPI.Models.AsyncStudyingNow> AsyncStudyingNowDBS { get; set; }

        public DbSet<SchoolPostponementAPI.Models.AsyncStudyYears> AsyncStudyYearsDBS { get; set; }
    }
}