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
    public class RabbitMQEndActiveCert : BackgroundService
    {
        private readonly CashAllowancLessThan42Context _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQEndActiveCert(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<CashAllowancLessThan42Context>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            startrabbitMQ();
            return Task.CompletedTask;
        }

        private void startrabbitMQ()
        {
            factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "EndActiveCert", ExchangeType.Fanout);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "EndActiveCert", routingKey: "");



            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                int CurrentID =Int32.Parse(recmess);
                CashAllowancLessThan42Model cashAllowancLessThan42Model = _context.CashAllowancLessThan42Db.Where(x=>x.UserID==CurrentID).OrderByDescending(x => x.DateOfGiven).FirstOrDefault();
                if (cashAllowancLessThan42Model != null)
                {
                    // cant do nothing
                }     
            };
            channel.BasicConsume(queue: queName,autoAck: true,consumer: consumer);
            System.Console.Read();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
