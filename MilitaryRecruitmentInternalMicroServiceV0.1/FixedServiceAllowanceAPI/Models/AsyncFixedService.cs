using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace FixedServiceAllowanceAPI.Models
{
    public class AsyncFixedService
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool fixedservice { get; set; }

        //public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
