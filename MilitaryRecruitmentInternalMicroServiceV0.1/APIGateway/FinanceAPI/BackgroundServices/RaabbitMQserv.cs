using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using FinanceAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using FinanceAPI.Data;
using System.Linq;

namespace FinanceAPI.BackgroundServices
{
    public class RaabbitMQserv : BackgroundService
    {
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        private readonly FinanceContext _context;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            Task.Run(async () =>
            {
                await startrabbitMQ();
            }, stoppingToken);
            return Task.CompletedTask;
        }
        public RaabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<FinanceContext>();
        }

        private Task startrabbitMQ()
        {
            this.factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();


            channel.QueueDeclare(queue: "UserCalcPayment", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var properties = channel.CreateBasicProperties();

                properties.CorrelationId = ea.BasicProperties.CorrelationId;

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                RabbitMQobj rabbitMQobj = System.Text.Json.JsonSerializer.Deserialize<RabbitMQobj>(recmess);

                OrderRef orderRef = new OrderRef();
                orderRef.PosponemtName = rabbitMQobj.URL;
                orderRef.ProcesID = rabbitMQobj.ProcID;
                _context.OrderRefDBS.Add(orderRef);
                _context.SaveChanges();
                
                
                    RabbitMQResponce rabbitMQResponce = new RabbitMQResponce();
                    rabbitMQResponce.RequestStatuseID = rabbitMQobj.RequestStatuseID;
                    rabbitMQResponce.ProcID = rabbitMQobj.ProcID;
                    rabbitMQResponce.Responce = GetExtraPayAamount(rabbitMQobj.URL, orderRef.OrderID).ToString() ;

                    var mess = System.Text.Json.JsonSerializer.Serialize(rabbitMQResponce);
                    var body = Encoding.UTF8.GetBytes(mess);

                    channel.BasicPublish("", ea.BasicProperties.ReplyTo, properties, body);
                
            };
            channel.BasicConsume(queue: "UserCalcPayment", autoAck: true, consumer: consumer);
            System.Console.Read();

            return null;
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
            string Verficode = CreateMD5(MerchID + MerchSecret + Amount.ToString() + orderRef);


            return new Uri(x1 + "/" + TerminalKey + "/" + MerchID + "/" + Verficode + "/" + Currency + "/" + Amount.ToString() + "/" + Lang + "/" + orderRef); ;
        }

        public Uri GetExtraPayAamount(string NameofService, int orderRef)
        {
            Finance Results = _context.FinanceContextDBS.Where(x => x.Name == NameofService).FirstOrDefault();
            Uri EcashURi = EcashCallURI(Results.Amount, orderRef.ToString());

            return EcashURi;
        }
    }
}
