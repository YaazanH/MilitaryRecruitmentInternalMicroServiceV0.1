using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AlonePostponement.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using AlonePostponement.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AlonePostponement.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly AlonePostponementContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }
        

        public RabbitMQserv(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<AlonePostponementContext>();
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "AlonePostponement");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                int processID = InsertRequestToDB(userInfo.UserID);
                SendToExternalAPI(userInfo.JWT, processID);
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
            //System.Console.ReadLine();
            return Task.CompletedTask;
        }

        private int InsertRequestToDB(int userID)
        {
            RequestStatues rs = new RequestStatues();
            rs.UserID = userID;
            rs.DateOfRecive = DateTime.Now;
            rs.Statues = "wating";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }



        public void SendToExternalAPI(String Token, int processID)
        {

            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();


            var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

            channel.QueueDeclare(queue: "requestQueue", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                RabbitMQResponce ExternalAPIResponce = JsonSerializer.Deserialize<RabbitMQResponce>(recmess);


                string CorrelationId = ea.BasicProperties.CorrelationId;


                UpdateRequestToDB(ExternalAPIResponce, CorrelationId);
            };

            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);


            var properties = channel.CreateBasicProperties();

            properties.ReplyTo = replyQueue.QueueName;
            for (int i = 0; i < 4; i++)
            {
                RabbitMQobj rabbitMQobj = new RabbitMQobj() { ProcID = processID, JWT = Token };
                if (i == 0)
                {
                    rabbitMQobj.URL = "";
                    properties.CorrelationId = "AlonepostmentBrothersID";
                }

                if (i == 1)
                {
                    rabbitMQobj.URL = "";
                    properties.CorrelationId = "AlonepostmentBrothersEill";
                }
                if (i == 2)
                {
                    rabbitMQobj.URL = "";
                    properties.CorrelationId = "AlonepostmentDeadBrothers";
                }
                if (i == 3)
                {
                    rabbitMQobj.URL = "";
                    properties.CorrelationId = "AlonepostmentHaveBrothers";
                }

                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(externalAPIResponce.ProcID);

            switch (correlationId)
            {
                case "AlonepostmentBrothersID":
                    
                    BrothersID brothersID = new BrothersID() { RequestStatuesID = requestStatues, BrotherID = externalAPIResponce.Responce, RequestReciveTime = DateTime.Now };
                    _context.BrothersIDDBS.Add(brothersID);
                    _context.SaveChanges();

                    break;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
