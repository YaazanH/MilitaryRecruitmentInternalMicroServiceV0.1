using System;
using System.ComponentModel.DataAnnotations;

namespace CashAllowanceAPI.Models
{
    public class CashAllowance
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
 
    }
}
