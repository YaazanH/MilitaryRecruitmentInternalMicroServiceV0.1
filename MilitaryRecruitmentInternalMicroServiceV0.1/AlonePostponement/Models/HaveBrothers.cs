using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class HaveBrothers
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatues { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool HaveBrother { get; set; }

        public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
