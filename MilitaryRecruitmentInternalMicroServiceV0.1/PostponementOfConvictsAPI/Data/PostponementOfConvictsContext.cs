using Microsoft.EntityFrameworkCore;

namespace PostponementOfConvictsAPI.Data
{
    public class PostponementOfConvictsContext : DbContext
    {
        public PostponementOfConvictsContext(DbContextOptions<PostponementOfConvictsContext> options) : base(options)
        { }

        public DbSet<PostponementOfConvictsAPI.Models.PostponementOfConvicts> PostponementOfConvictsDb { get; set; }
    }
}