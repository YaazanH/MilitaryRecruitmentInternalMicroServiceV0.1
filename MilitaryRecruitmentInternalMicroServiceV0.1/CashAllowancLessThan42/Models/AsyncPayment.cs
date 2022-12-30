using System;
using System.ComponentModel.DataAnnotations;

namespace CashAllowancLessThan42.Models
{
    public class AsyncPayment
    {
        [Key]
        public int PaymentID { get; set; }

        public virtual RequestStatues RequestStatuesID { get; set; }

        public DateTime RequestSendTime { get; set; }

        public bool Payed { get; set; }

        public double Amount { get; set; }

        public String EcashURl { get; set; }

        public String Statues { get; set; }

        public DateTime RequestReciveTime { get; set; }
    }
}
