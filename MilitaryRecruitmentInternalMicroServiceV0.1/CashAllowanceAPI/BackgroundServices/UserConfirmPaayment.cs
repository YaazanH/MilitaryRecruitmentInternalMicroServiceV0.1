using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CashAllowanceAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CashAllowanceAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FixedServiceAllowanceAPI.BackgroundServices
{
    public class UserConfirmPaayment : BackgroundService
    {
        private readonly CashAllowanceContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public UserConfirmPaayment(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<CashAllowanceContext>();
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

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "UserConfirmPay", routingKey: "CashAllowance");



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
            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "EndActiveCert", type: ExchangeType.Fanout);

                var message = UserID;
                var body = Encoding.UTF8.GetBytes(message.ToString());
                channel.BasicPublish(exchange: "EndActiveCert", routingKey: "", basicProperties: null, body: body);

            }
        }
        private void AddCert(int CUserID)
        {
            CashAllowance tra = new CashAllowance { UserID = CUserID, DateOfGiven = DateTime.Now };
            _context.CashAllowanceDb.Add(tra);
            _context.SaveChanges();

        }



    }
}
