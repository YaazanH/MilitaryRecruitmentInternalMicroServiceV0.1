﻿using Microsoft.EntityFrameworkCore;

namespace SchoolPostponementAPI.Models
{
    [Keyless]
    public class RabbitMQResponce
    {
        public int RequestStatuseID { get; set; }

        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
