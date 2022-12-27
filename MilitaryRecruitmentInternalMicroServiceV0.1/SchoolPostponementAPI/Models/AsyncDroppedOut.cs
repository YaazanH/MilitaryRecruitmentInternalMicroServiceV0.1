using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPostponementAPI.Models
{
    public class AsyncDroppedOut
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool IsDroppedOut { get; set; }

        public string statuse { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
