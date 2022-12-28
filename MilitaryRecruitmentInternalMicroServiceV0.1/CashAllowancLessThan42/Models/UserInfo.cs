using Microsoft.EntityFrameworkCore;

namespace CashAllowancLessThan42.Models
{
    [Keyless]
    public class UserInfo
    {
        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
