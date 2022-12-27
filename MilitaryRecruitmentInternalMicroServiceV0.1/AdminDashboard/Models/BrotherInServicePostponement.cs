using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminDashboard.Models
{
    public class BrotherInServicePostponement
    {
        [Key]
        public int id { set; get; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
        public DateTimeOffset DateOfEnd { get; set; }
    }
}
