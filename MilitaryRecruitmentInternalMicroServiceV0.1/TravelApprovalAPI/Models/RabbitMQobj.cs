using Microsoft.EntityFrameworkCore;

namespace TravelApprovalAPI.Models
{
    [Keyless]
    public class RabbitMQobj
    {
        public int ProcID { get; set; }

        public string URL { get; set; }

        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
