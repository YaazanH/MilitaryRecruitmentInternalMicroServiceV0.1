using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserTransactions.Models
{
    [Keyless]
    public class RequestStatues
    {
        public int ReqStatuesID { get; set; }

        public int UserID { get; set; }

        public string PostponmentType { get; set; }

        public DateTime DateOfRecive { get; set; }

        public DateTime DateOfDone { get; set; }

        public String Statues { get; set; }
    }
}
