using Microsoft.EntityFrameworkCore;
namespace CashAllowanceAPI.Data
{
    public class CashAllowanceContext : DbContext
    {
        public CashAllowanceContext(DbContextOptions<CashAllowanceContext> options) : base(options)
        { }

        public DbSet<CashAllowanceAPI.Models.CashAllowance> CashAllowanceDb { get; set; }

        public DbSet<CashAllowanceAPI.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }

        public DbSet<CashAllowanceAPI.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<CashAllowanceAPI.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<CashAllowanceAPI.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<CashAllowanceAPI.Models.AsyncAge> AsyncAgeDb { get; set; }

        public DbSet<CashAllowanceAPI.Models.AsyncUserTransactions> AsyncUserTransactionsDb { get; set; }
        public DbSet<CashAllowanceAPI.Models.AsyncPayment> AsyncPaymentDBS { get; set; }


    }
}