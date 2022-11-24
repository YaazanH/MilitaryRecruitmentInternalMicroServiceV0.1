using Microsoft.EntityFrameworkCore;
namespace CashAllowanceAPI.Data
{
    public class CashAllowanceContext : DbContext
    {
        public CashAllowanceContext(DbContextOptions<CashAllowanceContext> options) : base(options)
        { }

        public DbSet<CashAllowanceAPI.Models.CashAllowance> CashAllowanceDb { get; set; }

       
    }
}