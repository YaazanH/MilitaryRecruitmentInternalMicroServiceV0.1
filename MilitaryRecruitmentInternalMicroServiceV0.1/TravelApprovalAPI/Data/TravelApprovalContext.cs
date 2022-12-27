using Microsoft.EntityFrameworkCore;
namespace TravelApprovalAPI.Data
{
    public class TravelApprovalContext : DbContext
    {
        public TravelApprovalContext(DbContextOptions<TravelApprovalContext> options) : base(options)
        { }

        public DbSet<TravelApprovalAPI.Models.TravelApproval> TravelApprovalDb { get; set; }

      

        public DbSet<TravelApprovalAPI.Models.AsyncAge> AsyncAgeDBS { get; set; }

        public DbSet<TravelApprovalAPI.Models.Asynctravel> AsynctravelDBS { get; set; }
        public DbSet<TravelApprovalAPI.Models.AsyncUserTransactions> AsyncUserTransactionsDBS { get; set; }
        public DbSet<TravelApprovalAPI.Models.AsynLabor> AsynLaborDBS { get; set; }

        public DbSet<TravelApprovalAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<TravelApprovalAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<TravelApprovalAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<TravelApprovalAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }


    }
}