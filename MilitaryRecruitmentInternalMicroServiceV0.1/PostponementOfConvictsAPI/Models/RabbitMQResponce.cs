using Microsoft.EntityFrameworkCore;

namespace PostponementOfConvictsAPI.Models
{
    [Keyless]
    public class RabbitMQResponce
    {

        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
