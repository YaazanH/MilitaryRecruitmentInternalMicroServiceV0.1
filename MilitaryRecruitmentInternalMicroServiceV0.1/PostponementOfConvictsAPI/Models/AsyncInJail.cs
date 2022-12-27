using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace PostponementOfConvictsAPI.Models
{
    public class AsyncInJail
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool InJail { get; set; }

       // public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
