using System;
using System.ComponentModel.DataAnnotations;

namespace TravelApprovalAPI.Models
{
    public class TravelApproval
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
        public DateTimeOffset DateOfEnd { get; set; }
    }
}
