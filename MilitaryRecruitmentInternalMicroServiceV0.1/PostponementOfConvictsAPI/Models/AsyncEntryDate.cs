using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace PostponementOfConvictsAPI.Models
{
    public class AsyncEntryDate
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public DateTime Entrydate { get; set; }

       // public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
