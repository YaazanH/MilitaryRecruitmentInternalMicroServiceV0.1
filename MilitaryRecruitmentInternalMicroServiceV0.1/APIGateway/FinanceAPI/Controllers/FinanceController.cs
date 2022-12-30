using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FinanceAPI.Models;
using FinanceAPI.Data;
using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using RabbitMQ.Client;


namespace FinanceAPI.Controllers
{
    [ApiController]
    [Route("Finance")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FinanceController : Controller
    {
        private readonly FinanceContext _context;
        public FinanceController(FinanceContext context)
        {
            _context = context;
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);

            }
        }

        public Uri EcashCallURI(double Amount, string orderRef)
        {
            string x1 = "https://checkout.ecash-pay.co/Checkout/Card";
            string TerminalKey = "GB97ST";
            string MerchID = "IXZM1E";
            string MerchSecret = "70JWCXM60MX7A32ENV03XHQBD5FF4VSQIJV3KUUV5HFP0Y5M9DJDV8QZKHSGRHBE";
            string Currency = "SYP";
            string Lang = "AR";
            string Verficode = CreateMD5(MerchID+MerchSecret+Amount.ToString()+orderRef);


            return new Uri(x1+ "/" +TerminalKey +"/" +MerchID+"/"+ Verficode + "/"+Currency + "/" + Amount.ToString()+ "/" + Lang + "/" + orderRef); ;
        }

        [HttpGet]
        [Route("GetExtraPayAamount")]
        public Uri GetExtraPayAamount(string NameofService, string orderRef)
        {
           Finance Results = _context.FinanceContextDBS.Where(x=>x.Name==NameofService).FirstOrDefault();
           Uri EcashURi= EcashCallURI(Results.Amount,orderRef);

           return EcashURi;
        }

        [HttpGet]
        [Route("GetPayAamount")]
        public Uri GetPayAamount(string NameofService)
        {
            Finance Results = _context.FinanceContextDBS.Where(x => x.Name == NameofService).FirstOrDefault();
            Uri EcashURi = EcashCallURI(Results.Amount,"0");

            return EcashURi;
        }


        [HttpGet]
        [Route("GetConfirmPay")]
        public void GetConfirmPay([FromBody]ConfirmPay confirmPay)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserConfirmPay", type: ExchangeType.Direct);

            OrderRef orderRef= _context.OrderRefDBS.Where(x=>x.OrderID==Int32.Parse( confirmPay.ConfirmPayOrderRef)).FirstOrDefault();
            var mess = orderRef.ProcesID;
            var body = Encoding.UTF8.GetBytes(mess.ToString());

            string RoueKey = "";

            switch (orderRef.PosponemtName)
            {
                case "FixedServiceAllowance":
                    RoueKey = "FixedServiceAllowance";
                    break;

                case "CashAllowancLessThan42":
                    RoueKey = "CashAllowancLessThan42";
                    break;

                case "42":
                    RoueKey = "CashAllowance";
                    break;

            }
            channel.BasicPublish("UserConfirmPay", RoueKey, null, body);

            
        }
    }
}
