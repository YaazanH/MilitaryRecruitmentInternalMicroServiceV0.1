using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models
{
    [Keyless]
    public class CashAllowance
    {
        
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
 
    }
}
