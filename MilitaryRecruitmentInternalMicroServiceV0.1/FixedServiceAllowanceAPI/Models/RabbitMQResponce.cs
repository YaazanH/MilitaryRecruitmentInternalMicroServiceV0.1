using Microsoft.EntityFrameworkCore;

namespace FixedServiceAllowanceAPI.Models
{
    [Keyless]
    public class RabbitMQResponce
    {

        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
