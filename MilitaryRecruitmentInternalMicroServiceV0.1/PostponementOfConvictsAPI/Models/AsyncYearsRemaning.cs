using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace PostponementOfConvictsAPI.Models
{
    public class AsyncYearsRemaning
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public int Years { get; set; }

        //public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
