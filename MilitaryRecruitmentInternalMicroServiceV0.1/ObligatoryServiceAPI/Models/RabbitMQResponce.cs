using Microsoft.EntityFrameworkCore;

namespace ObligatoryServiceAPI.Models
{
    [Keyless]
    public class RabbitMQResponce
    {

        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
