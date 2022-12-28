using Microsoft.EntityFrameworkCore;

namespace ExternalAPICaller.Models
{
    [Keyless]
    public class RabbitMQResponce
    {

        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
