using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPostponementAPI.Models
{
    public class AsyncStudyingNow
    {
        [Key]

        public int ID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool StudyingNow { get; set; }

        //public IActionResult Result { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
