﻿using Microsoft.EntityFrameworkCore;

namespace CashAllowanceAPI.Models
{
    [Keyless]
    public class RabbitMQResponce
    {

        public int RequestStatuseID { get; set; }


        public int ProcID { get; set; }
        public string Responce { get; set; }

    }
}
