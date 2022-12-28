using Microsoft.EntityFrameworkCore;

namespace BrotherInServicePostponementAPI.Models
{
    [Keyless]

    public class UserInfo
    {
        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
