using Microsoft.EntityFrameworkCore;

namespace BrotherInServicePostponementAPI.Models
{
    [Keyless]

    public class RabbitMQobj
    {
        public int RequestStatuseID { get; set; }

        public int ProcID { get; set; }

        public string URL { get; set; }

        public int UserID { get; set; }

        public string JWT { get; set; }
    }
}
