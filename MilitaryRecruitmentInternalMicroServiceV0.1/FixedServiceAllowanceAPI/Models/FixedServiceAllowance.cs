using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FixedServiceAllowanceAPI.Models
{
    public class FixedServiceAllowance
    {
        [Key]
        public int id { set; get; }
        public string FullName { set; get; }
      
        public DateTimeOffset DateOfGiven { get; set; }
        


    }
}
