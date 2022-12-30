using Microsoft.EntityFrameworkCore;

namespace FixedServiceAllowanceAPI.Data
{
    public class FixedServiceAllowanceContext : DbContext
    {
        public FixedServiceAllowanceContext(DbContextOptions<FixedServiceAllowanceContext> options) : base(options)
        { }

        public DbSet<FixedServiceAllowanceAPI.Models.FixedServiceAllowance> FixedServiceAllowanceContextDBS { get; set; }

        public DbSet<FixedServiceAllowanceAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<FixedServiceAllowanceAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<FixedServiceAllowanceAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<FixedServiceAllowanceAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<FixedServiceAllowanceAPI.Models.AsyncFixedService> AsyncFixedServiceDB { get; set; }
        public DbSet<FixedServiceAllowanceAPI.Models.AsyncPayment> AsyncPaymentDBS { get; set; }
    }
}