using Microsoft.EntityFrameworkCore;
namespace CashAllowanceAPI.Data
{
    public class CashAllowanceContext : DbContext
    {
        public CashAllowanceContext(DbContextOptions<CashAllowanceContext> options) : base(options)
        { }

        public DbSet<CashAllowanceAPI.Models.CashAllowance> CashAllowanceDb { get; set; }

        public DbSet<CashAllowanceAPI.Models.RequestStatues> RequestStatuesDB { get; set; }
        public DbSet<CashAllowanceAPI.Models.RabbitMQResponce> RabbitMQResponceDB { get; set; }
        public DbSet<CashAllowanceAPI.Models.RabbitMQobj> RabbitMQobjDB { get; set; }
        public DbSet<CashAllowanceAPI.Models.UserInfo> UserInfoDB { get; set; }

        public DbSet<CashAllowanceAPI.Models.Age> AgeDB { get; set; }
        public DbSet<CashAllowanceAPI.Models.FinancialyClear> FinancialyClearDB { get; set; }
    }
}