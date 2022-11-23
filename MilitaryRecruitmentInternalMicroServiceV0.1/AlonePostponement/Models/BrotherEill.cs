using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class BrotherEill
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatues { get; set; }

        public DateTime RequestSendTime { get; set; }

        //ask if all brother eill dot care if one is not 
        public bool AllBrotherEill { get; set; }

        public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
