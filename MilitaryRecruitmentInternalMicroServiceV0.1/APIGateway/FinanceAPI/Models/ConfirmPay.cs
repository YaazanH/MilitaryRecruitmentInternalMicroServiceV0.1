using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Models
{
    [Keyless]
    public class ConfirmPay
    {
        public string ConfirmPayOrderRef { get; set; }

    }
}
