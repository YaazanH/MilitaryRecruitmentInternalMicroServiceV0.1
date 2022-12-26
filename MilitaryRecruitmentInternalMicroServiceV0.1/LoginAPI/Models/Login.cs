using System.ComponentModel.DataAnnotations;

namespace LoginAPI.Models
{
    public class Login
    {
        [Key]
        public int ID { get; set; }

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
