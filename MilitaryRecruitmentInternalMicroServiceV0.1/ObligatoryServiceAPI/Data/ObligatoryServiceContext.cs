using Microsoft.EntityFrameworkCore;

namespace ObligatoryServiceAPI.Data
{
    public class ObligatoryServiceContext : DbContext
    {
        public ObligatoryServiceContext(DbContextOptions<ObligatoryServiceContext> options) : base(options)
        { }

        public DbSet<ObligatoryServiceAPI.Models.ObligatoryService> ObligatoryServiceDB { get; set; }
        public DbSet<ObligatoryServiceAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<ObligatoryServiceAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<ObligatoryServiceAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<ObligatoryServiceAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<ObligatoryServiceAPI.Models.AsyncDonatedBlood> AsyncDonatedBloodDB { get; set; }
    }
}