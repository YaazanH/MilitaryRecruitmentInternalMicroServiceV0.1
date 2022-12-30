using System.ComponentModel.DataAnnotations;

namespace FinanceAPI.Models
{
    public class OrderRef
    {
        [Key]
        public int OrderID { get; set; }

        public string  PosponemtName { get; set; }

        public int ProcesID { get; set; }
    }
}
