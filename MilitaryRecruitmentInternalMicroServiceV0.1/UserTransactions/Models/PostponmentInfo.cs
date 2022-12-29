using Microsoft.EntityFrameworkCore;

namespace UserTransactions.Models
{
    [Keyless]
    public class PostponmentInfo
    {
        public int PosponmentID { get; set; }

        public string  PosponmentName { get; set; }

    }
}
