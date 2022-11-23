using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class RequestStatues
    {
        [Key]
        public int ID { get; set; }

        public int UserID { get; set; }

        public DateTime DateOfRecive { get; set; }

        public String Statues { get; set; }
    }
}
