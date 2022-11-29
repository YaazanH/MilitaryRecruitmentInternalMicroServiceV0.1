using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace TravelApprovalAPI.Models
{
    public class Asynctravel
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool travel { get; set; }

       // public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
