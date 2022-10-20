using Microsoft.EntityFrameworkCore;
namespace TravelApprovalAPI.Data
{
    public class TravelApprovalContext : DbContext
    {
        public TravelApprovalContext(DbContextOptions<TravelApprovalContext> options) : base(options)
        { }

        public DbSet<TravelApprovalAPI.Models.TravelApproval> TravelApprovalDb { get; set; }
    }
}