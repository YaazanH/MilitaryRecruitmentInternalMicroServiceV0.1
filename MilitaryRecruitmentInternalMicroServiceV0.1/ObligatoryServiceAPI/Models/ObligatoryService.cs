﻿using System.ComponentModel.DataAnnotations;
using System;

namespace ObligatoryServiceAPI.Models
{
    public class ObligatoryService
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset DateOfGiven { get; set; }
        public DateTimeOffset DateOfEnd { get; set; }
    }
}
