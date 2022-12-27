using Microsoft.EntityFrameworkCore;

namespace PostponementOfConvictsAPI.Data
{
    public class PostponementOfConvictsContext : DbContext
    {
        public PostponementOfConvictsContext(DbContextOptions<PostponementOfConvictsContext> options) : base(options)
        { }

        public DbSet<PostponementOfConvictsAPI.Models.PostponementOfConvicts> PostponementOfConvictsDb { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.AsyncEntryDate> AsyncEntryDateDb { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.AsyncInJail> AsyncInJailDb { get; set; }

        public DbSet<PostponementOfConvictsAPI.Models.AsyncYearsRemaning> AsyncYearsRemaningDb { get; set; }
    }
}