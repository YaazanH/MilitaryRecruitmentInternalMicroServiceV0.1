﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CashAllowancLessThan42.Models
{
    public class CashAllowancLessThan42Model
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }

    }
}
