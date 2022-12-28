using Microsoft.EntityFrameworkCore;

namespace FixedServiceAllowanceAPI.Models
{
    [Keyless]
    public class UserInfo
    {
        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
