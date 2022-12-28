using Microsoft.EntityFrameworkCore;

namespace UserRequestHandler.Models
{
    [Keyless]
    public class UserInfo
    {
        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
