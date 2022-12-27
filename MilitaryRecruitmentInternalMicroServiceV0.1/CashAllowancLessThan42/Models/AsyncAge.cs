﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace CashAllowancLessThan42.Models
{
    public class AsyncAge
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public int Age { get; set; }

        public string statuse{ get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
