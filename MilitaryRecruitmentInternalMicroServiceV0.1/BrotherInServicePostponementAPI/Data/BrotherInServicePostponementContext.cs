using Microsoft.EntityFrameworkCore;
namespace SchoolPostponementAPI.Data
{
    public class BrotherInServicePostponementContext : DbContext
    {
        public BrotherInServicePostponementContext(DbContextOptions<BrotherInServicePostponementContext> options) : base(options)
        { }

        public DbSet<BrotherInServicePostponementAPI.Models.BrotherInServicePostponement> BrotherInServicePostponementDBS { get; set; }
        public DbSet<BrotherInServicePostponementAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<BrotherInServicePostponementAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<BrotherInServicePostponementAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<BrotherInServicePostponementAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }
    }
}