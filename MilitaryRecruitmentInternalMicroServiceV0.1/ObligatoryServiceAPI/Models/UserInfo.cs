using Microsoft.EntityFrameworkCore;

namespace ObligatoryServiceAPI.Models
{
    [Keyless]
    public class UserInfo
    {
        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
