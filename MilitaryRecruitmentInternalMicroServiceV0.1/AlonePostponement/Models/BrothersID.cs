﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class BrothersID
    {
        [Key]
        
        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime{ get; set; }

        public String BrotherID { get; set; }

        public int NumbersOfBrothers { get; set; }

        public string Statues { get; set; }

        public DateTime RequestReciveTime { get; set; }

        


    }
}
