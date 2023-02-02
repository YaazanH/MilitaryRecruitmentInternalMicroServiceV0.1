using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CashAllowancLessThan42.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CashAllowancLessThan42.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CashAllowancLessThan42.BackgroundServices
{
    public class UserConfirmPaayment : BackgroundService
    {
        private readonly CashAllowancLessThan42Context _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public UserConfirmPaayment(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<CashAllowancLessThan42Context>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            Task.Run(async () =>
            {
                await startrabbitMQ();
            }, stoppingToken);
            return Task.CompletedTask;
        }

        private Task startrabbitMQ()
        {
            factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserConfirmPay", ExchangeType.Fanout);

            var queName = channel.QueueDeclare(queue: "cashallowConf", durable: true, autoDelete: false, exclusive: false, arguments: null);

            channel.QueueBind(queue: queName, exchange: "UserConfirmPay", routingKey: "CashAllowancLessThan42");



            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                int ProcessID = Int32.Parse(recmess);
                RequestStatues requestStatues  = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == ProcessID).FirstOrDefault();
                if (requestStatues != null)
                {
                    AsyncPayment asyncPayment = _context.AsyncPaymentDBS.Where(x=>x.RequestStatuesID == requestStatues).FirstOrDefault();
                    asyncPayment.Payed = true;
                    asyncPayment.EcashURl = "";
                    asyncPayment.RequestReciveTime = DateTime.Now;
                    asyncPayment.Statues = "Done";
                    _context.AsyncPaymentDBS.Update(asyncPayment);
                    _context.SaveChanges();

                    EndOtherPostponment(requestStatues.UserID);

                    AddCert(requestStatues.UserID);
                }
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
            System.Console.Read();
            return null;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }


        private void EndOtherPostponment(int UserID)
        {
           /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())*/
            
                channel.ExchangeDeclare(exchange: "EndActiveCert", type: ExchangeType.Fanout);

                var message = UserID;
                var body = Encoding.UTF8.GetBytes(message.ToString());
            var prop = channel.CreateBasicProperties();
            prop.Persistent = true;
            channel.BasicPublish(exchange: "EndActiveCert", routingKey: "", prop, body: body);


        }

        private void AddCert(int CUserID)
        {
            CashAllowancLessThan42Model tra = new CashAllowancLessThan42Model { UserID = CUserID, DateOfGiven = DateTime.Now };
            _context.CashAllowancLessThan42Db.Add(tra);
            _context.SaveChanges();

        }



    }
}
