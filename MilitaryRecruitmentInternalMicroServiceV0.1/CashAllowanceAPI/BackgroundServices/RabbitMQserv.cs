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

namespace CashAllowanceAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly CashAllowanceContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQserv(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<CashAllowanceContext>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            Task.Run(async () =>
            {
                await startrabbitMQ();
            }, stoppingToken); return Task.CompletedTask;
        }

        private Task startrabbitMQ()
        {
                factory = new ConnectionFactory() { HostName = "host.docker.internal" };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

                var queName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "CashAllowance");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {

                    var recbody = ea.Body.ToArray();

                    var recmess = Encoding.UTF8.GetString(recbody);

                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                    var User = _context.CashAllowanceDb.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
                    if (User == null)
                    {

                            int ReqStatuesID = InsertRequestToDB(userInfo.UserID);
                            SendToExternalAPI(userInfo.JWT, ReqStatuesID);
                        
                    }
                };
                channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
            System.Console.Read();
            return null;
        }

        private int InsertRequestToDB(int userID)
        {
            RequestStatues rs = new RequestStatues();
            rs.UserID = userID;
            rs.DateOfRecive = DateTime.Now;
            rs.Statues = "wating";
            rs.PostponmentType = "CashAllowance";
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
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    AsyncAge asyncage = new AsyncAge() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now,Statues="wating" };
                    _context.AsyncAgeDb.Add(asyncage);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://host.docker.internal:40018/RecordAdminstration/GetAge";
                    properties.CorrelationId = "GetAge";
                    rabbitMQobj.ProcID = asyncage.ID;
                }

                if (i == 1)
                {
                    AsyncUserTransactions asyncUserTransactions = new AsyncUserTransactions() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now,Statues="wating" };
                    _context.AsyncUserTransactionsDb.Add(asyncUserTransactions);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://host.docker.internal:40022/Finance/GetUserTransactions";
                    properties.CorrelationId = "GetUserTransactions";
                    rabbitMQobj.ProcID = asyncUserTransactions.ID;
                }
             
                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId, int processID)
        {
            switch (correlationId)
            {
        
                case "GetUserTransactions":

                    AsyncUserTransactions asyncUserTransactions = _context.AsyncUserTransactionsDb.Find(externalAPIResponce.ProcID);

                    asyncUserTransactions.UserTransactions = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncUserTransactions.RequestReciveTime = DateTime.Now;
                    asyncUserTransactions.Statues = "Done";      
                    _context.AsyncUserTransactionsDb.Update(asyncUserTransactions);
                    _context.SaveChanges();

                    break;
       
                case "GetAge":

                    AsyncAge asyncAge = _context.AsyncAgeDb.Find(externalAPIResponce.ProcID);
                    asyncAge.Age = 1;// Int32.Parse(externalAPIResponce.Responce);
                    asyncAge.RequestReciveTime = DateTime.Now;
                    asyncAge.Statues = "Done";
                    _context.AsyncAgeDb.Update(asyncAge);
                    _context.SaveChanges();

                    break;
            }
            CheckIfFinish(processID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
          
            AsyncUserTransactions asyncUserTransactions = _context.AsyncUserTransactionsDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
           
            AsyncAge asyncAge = _context.AsyncAgeDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;


            if (asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncUserTransactions.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
            {

                requestStatues.DateOfDone = DateTime.Now;
                requestStatues.Statues = "Done";
                _context.RequestStatuesDBS.Update(requestStatues);
                _context.SaveChanges();
                if (asyncAge.Age > 42)
                {


                    if (asyncUserTransactions.UserTransactions)
                    {

                        EndOtherPostponment(requestStatues.UserID);

                        AddCert(requestStatues.UserID);

                    }

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
            CashAllowance tra = new CashAllowance { UserID = CUserID, DateOfGiven = DateTime.Now};
            _context.CashAllowanceDb.Add(tra);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
