using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FixedServiceAllowanceAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using FixedServiceAllowanceAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FixedServiceAllowanceAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly FixedServiceAllowanceContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQserv(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<FixedServiceAllowanceContext>();
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

            channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "FixedServiceAllowance");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                var User = _context.FixedServiceAllowanceContextDBS.Where(x => x.UserId == userInfo.UserID).FirstOrDefault();
                if (User == null)
                {
                    int ReqStatuesID = InsertRequestToDB(userInfo.UserID);
                    SendToExternalAPI(userInfo.JWT, ReqStatuesID);
                }
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
            //Console.ReadLine(); 

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



        public void SendToExternalAPI(String Token, int ReqStatuesID)
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


                UpdateRequestToDB(ExternalAPIResponce, CorrelationId, ReqStatuesID);
            };

            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);


            var properties = channel.CreateBasicProperties();

            properties.ReplyTo = replyQueue.QueueName;
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(ReqStatuesID);
            RabbitMQobj rabbitMQobj = new RabbitMQobj() { JWT = Token };

            AsyncFixedService asyncFixedService = new AsyncFixedService() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
            _context.AsyncFixedServiceDB.Add(asyncFixedService);
            _context.SaveChanges();
            rabbitMQobj.URL = "https://host.docker.internal:40013/DefenseAPI/GetIsFixed";
            properties.CorrelationId = "GetIsFixedService";
            rabbitMQobj.ProcID = asyncFixedService.ID;





            var mess = JsonSerializer.Serialize(rabbitMQobj);
            var body = Encoding.UTF8.GetBytes(mess);

            channel.BasicPublish("", "requestQueue", properties, body);

            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId, int processID)
        {
            switch (correlationId)
            {
                case "GetIsFixedService":

                    AsyncFixedService asynctravel = _context.AsyncFixedServiceDB.Find(externalAPIResponce.ProcID);

                    asynctravel.fixedservice = true;// bool.Parse(externalAPIResponce.Responce);
                    asynctravel.RequestReciveTime = DateTime.Now;
                    _context.AsyncFixedServiceDB.Update(asynctravel);
                    _context.SaveChanges();

                    break;

            }
            CheckIfFinish(processID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            AsyncFixedService asyncFixedService = _context.AsyncFixedServiceDB.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;

            if (asyncFixedService.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
            {
                requestStatues.DateOfDone = DateTime.Now;
                requestStatues.Statues = "Done";
                _context.RequestStatuesDBS.Update(requestStatues);
                _context.SaveChanges();



                if (asyncFixedService.fixedservice)
                {
                    EndOtherPostponment(requestStatues.UserID);

                    AddCert(requestStatues.UserID);

                }

            }
            else
            {
                requestStatues.DateOfDone = DateTime.Now;
                requestStatues.Statues = "Faild";
                _context.RequestStatuesDBS.Update(requestStatues);
                _context.SaveChanges();
            }
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
            FixedServiceAllowance tra = new FixedServiceAllowance { UserId = CUserID, DateOfGiven = DateTime.Now };
            _context.FixedServiceAllowanceContextDBS.Add(tra);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}