using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolPostponementAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using SchoolPostponementAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SchoolPostponementAPI.BackgroundServices
{
    public class RabbitMQEndActiveCert : BackgroundService
    {
        private readonly SchoolPostponementContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQEndActiveCert(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<SchoolPostponementContext>();
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

            channel.ExchangeDeclare(exchange: "EndActiveCert", ExchangeType.Fanout);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "EndActiveCert", routingKey: "");



            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                int CurrentID =Int32.Parse(recmess);
                SchoolPostponement schoolPostponement = _context.schoolDBS.Where(x=>x.UserID==CurrentID).OrderByDescending(x => x.DateOfEnd).FirstOrDefault();
                if (schoolPostponement != null)
                {
                    schoolPostponement.DateOfEnd = DateTime.Now;
                    _context.Update(schoolPostponement);
                }
            };
            channel.BasicConsume(queue: queName,autoAck: true,consumer: consumer);
            System.Console.Read();
            return null;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
