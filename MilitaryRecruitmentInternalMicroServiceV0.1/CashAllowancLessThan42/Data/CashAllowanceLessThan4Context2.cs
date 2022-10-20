using Microsoft.EntityFrameworkCore;
namespace CashAllowancLessThan42.Data
{
    public class CashAllowancLessThan42Context : DbContext
    {
        public CashAllowancLessThan42Context(DbContextOptions<CashAllowancLessThan42Context> options) : base(options)
        { }

        public DbSet<CashAllowancLessThan42.Models.CashAllowancLessThan42Model> CashAllowancLessThan42Db { get; set; }
    }
}