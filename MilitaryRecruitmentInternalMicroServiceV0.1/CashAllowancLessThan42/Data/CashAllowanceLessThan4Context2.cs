using Microsoft.EntityFrameworkCore;
namespace CashAllowancLessThan42.Data
{
    public class CashAllowancLessThan42Context : DbContext
    {
        public CashAllowancLessThan42Context(DbContextOptions<CashAllowancLessThan42Context> options) : base(options)
        { }

        public DbSet<CashAllowancLessThan42.Models.CashAllowancLessThan42Model> CashAllowancLessThan42Db { get; set; }

        public DbSet<CashAllowancLessThan42.Models.RequestStatues> RequestStatuesDBS { get; set; }

        public DbSet<CashAllowancLessThan42.Models.AsyncAge> AsyncAgeDBS { get; set; }
        public DbSet<CashAllowancLessThan42.Models.AsyncDaysOutsideCoun> AsyncDaysOutsideCounDBS { get; set; }
        public DbSet<CashAllowancLessThan42.Models.Asynctravel> AsynctravelDBS { get; set; }
        public DbSet<CashAllowancLessThan42.Models.AsyncUserTransactions> AsyncUserTransactionsDBS { get; set; }

        public DbSet<CashAllowancLessThan42.Models.RabbitMQResponce> RabbitMQResponceDBS { get; set; }

        public DbSet<CashAllowancLessThan42.Models.UserInfo> UserInfoDBS { get; set; }

        public DbSet<CashAllowancLessThan42.Models.RabbitMQobj> RabbitMQobjDBS { get; set; }
        public DbSet<CashAllowancLessThan42.Models.AsyncPayment> AsyncPaymentDBS { get; set; }


    }
}