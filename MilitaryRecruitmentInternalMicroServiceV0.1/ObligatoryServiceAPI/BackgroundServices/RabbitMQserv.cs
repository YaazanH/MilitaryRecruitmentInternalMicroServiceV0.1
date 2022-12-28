using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ObligatoryServiceAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ObligatoryServiceAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ObligatoryServiceAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly ObligatoryServiceContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQserv(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<ObligatoryServiceContext>();
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

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "ObligatoryService");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                var User = _context.ObligatoryServiceDB.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
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





            AsyncDonatedBlood asyncDonatedBlood = new AsyncDonatedBlood() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
            _context.AsyncDonatedBloodDB.Add(asyncDonatedBlood);
            _context.SaveChanges();
            rabbitMQobj.URL = "https://host.docker.internal:40005/BloodBank/HasDonated";
            properties.CorrelationId = "GetHasDonated";
            rabbitMQobj.ProcID = asyncDonatedBlood.ID;



            var mess = JsonSerializer.Serialize(rabbitMQobj);
            var body = Encoding.UTF8.GetBytes(mess);

            channel.BasicPublish("", "requestQueue", properties, body);

            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId, int processID)
        {
            switch (correlationId)
            {
                case "GetHasDonated":

                    AsyncDonatedBlood asyncDonatedBlood = _context.AsyncDonatedBloodDB.Find(externalAPIResponce.ProcID);

                    asyncDonatedBlood.Donated = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncDonatedBlood.RequestReciveTime = DateTime.Now;
                    _context.AsyncDonatedBloodDB.Update(asyncDonatedBlood);
                    _context.SaveChanges();

                    break;

            }
            CheckIfFinish(processID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            AsyncDonatedBlood asyncDonatedBlood = _context.AsyncDonatedBloodDB.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;
            if (asyncDonatedBlood.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
            {

                requestStatues.DateOfDone = DateTime.Now;
                requestStatues.Statues = "Done";
                _context.RequestStatuesDBS.Update(requestStatues);
                _context.SaveChanges();




                if (asyncDonatedBlood.Donated)
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
            ObligatoryService tra = new ObligatoryService { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
            _context.ObligatoryServiceDB.Add(tra);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
