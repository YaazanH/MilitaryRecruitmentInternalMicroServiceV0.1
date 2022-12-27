using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminDashboard.Models
{
    public class FixedServiceAllowance
    {
        [Key]
        public int id { set; get; }
        public int UserId { set; get; }

        public DateTimeOffset DateOfGiven { get; set; }
        


    }
}
