using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class AlonePostponement
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
        public DateTimeOffset DateOfEnd { get; set; }
    }
}
