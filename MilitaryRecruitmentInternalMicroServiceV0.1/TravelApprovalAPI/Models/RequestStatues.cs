using System;
using System.ComponentModel.DataAnnotations;

namespace TravelApprovalAPI.Models
{
    public class RequestStatues
    {
        [Key]
        public int ReqStatuesID { get; set; }

        public string PostponmentType { get; set; }

        public int UserID { get; set; }

        public DateTime DateOfRecive { get; set; }

        public DateTime DateOfDone { get; set; }

        public String Statues { get; set; }
    }
}
