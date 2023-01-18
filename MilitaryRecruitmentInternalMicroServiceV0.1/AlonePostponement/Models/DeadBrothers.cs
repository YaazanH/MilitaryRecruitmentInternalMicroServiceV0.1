using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlonePostponement.Models
{
    public class DeadBrothers
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        //check if all brothers are dead
        public bool AllDeadBrothers { get; set; }

        public string Statues { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
