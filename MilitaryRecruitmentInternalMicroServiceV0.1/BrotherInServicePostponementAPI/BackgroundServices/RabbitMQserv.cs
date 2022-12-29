//using System.Threading.Tasks;
//using Microsoft.Extensions.Options;
//using System.Threading;
//using System;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using CashAllowancLessThan42.Data;
//using RabbitMQ.Client;
//using Microsoft.AspNetCore.Mvc;
//using System.Text;
//using CashAllowancLessThan42.Models;
//using RabbitMQ.Client.Events;
//using System.Text.Json;
//using Microsoft.Extensions.DependencyInjection;
//using System.Linq;

//namespace BrotherInServicePostponementAPI.BackgroundServices
//{
//    public class RabbitMQserv : BackgroundService
//    {
//        private readonly CashAllowancLessThan42Context _context;
//        ConnectionFactory factory { get; set; }
//        IConnection connection { get; set; }
//        IModel channel { get; set; }

//        public RabbitMQserv(IServiceScopeFactory factory)
//        {
//            _context = factory.CreateScope().ServiceProvider.GetRequiredService<CashAllowancLessThan42Context>();
//        }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();
//                       Task.Run(async () =>
//{
//    await startrabbitMQ();
//}, stoppingToken);
//            return Task.CompletedTask;
//        }

//        private Task startrabbitMQ()
//        {
//                factory = new ConnectionFactory() { HostName = "host.docker.internal" };
//                connection = factory.CreateConnection();
//                channel = connection.CreateModel();

//                channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

//                var queName = channel.QueueDeclare().QueueName;

//                channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "BrotherInServicePostponement");

//                var consumer = new EventingBasicConsumer(channel);

//                consumer.Received += (model, ea) =>
//                {

//                    var recbody = ea.Body.ToArray();

//                    var recmess = Encoding.UTF8.GetString(recbody);

//                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

//                    var User = _context.CashAllowancLessThan42Db.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
//                    if (User == null)
//                    {
//                        int ReqStatuesID = InsertRequestToDB(userInfo.UserID);
//                        SendToExternalAPI(userInfo.JWT, ReqStatuesID);
//                    }                    
//                };
//                channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
//                                System.Console.Read(); 
//return null;

//        }

//        private int InsertRequestToDB(int userID)
//        {
//            RequestStatues rs = new RequestStatues();
//            rs.UserID = userID;
//            rs.DateOfRecive = DateTime.Now;
//            rs.Statues = "wating";
//rs.PostponmentType = "BrotherInServicePostponement";

//            _context.RequestStatuesDBS.Add(rs);
//            _context.SaveChanges();

//            return rs.ReqStatuesID;
//        }



//        public void SendToExternalAPI(String Token, int ReqStatuesID)
//        {

//            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

//            using var connection = factory.CreateConnection();

//            using var channel = connection.CreateModel();


//            var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

//            channel.QueueDeclare(queue: "requestQueue", exclusive: false);

//            var consumer = new EventingBasicConsumer(channel);

//            consumer.Received += (model, ea) =>
//            {
//                var recbody = ea.Body.ToArray();

//                var recmess = Encoding.UTF8.GetString(recbody);

//                RabbitMQResponce ExternalAPIResponce = JsonSerializer.Deserialize<RabbitMQResponce>(recmess);


//                string CorrelationId = ea.BasicProperties.CorrelationId;


//                UpdateRequestToDB(ExternalAPIResponce, CorrelationId, ReqStatuesID);
//            };

//            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);


//            var properties = channel.CreateBasicProperties();

//            properties.ReplyTo = replyQueue.QueueName;
//            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(ReqStatuesID);
//            RabbitMQobj rabbitMQobj = new RabbitMQobj() { JWT = Token };
//            for (int i = 0; i < 4; i++)
//            {
//                if (i == 0)
//                {
//                    Asynctravel asynctravel = new Asynctravel() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now };
//                    _context.AsynctravelDBS.Add(asynctravel);
//                    _context.SaveChanges();
//                    rabbitMQobj.URL = "https://host.docker.internal:40011/Passport/GetIstravel";
//                    properties.CorrelationId = "GetIstravel";
//                    rabbitMQobj.ProcID = asynctravel.ID;
//                }

//                if (i == 1)
//                {
//                    AsyncUserTransactions asyncUserTransactions = new AsyncUserTransactions() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
//                    _context.AsyncUserTransactionsDBS.Add(asyncUserTransactions);
//                    _context.SaveChanges();
//                    rabbitMQobj.URL = "https://host.docker.internal:40022/Finance/GetUserTransactions";
//                    properties.CorrelationId = "GetUserTransactions";
//                    rabbitMQobj.ProcID = asyncUserTransactions.ID;
//                }
//                if (i == 2)
//                {
//                    AsyncDaysOutsideCoun asyncDaysOutsideCoun = new AsyncDaysOutsideCoun() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
//                    _context.AsyncDaysOutsideCounDBS.Add(asyncDaysOutsideCoun);
//                    _context.SaveChanges();
//                    rabbitMQobj.ProcID = asyncDaysOutsideCoun.ID;
//                    rabbitMQobj.URL = "https://host.docker.internal:40011/Passport/GetNumberOfDaysOutsideCoun";
//                    properties.CorrelationId = "GetNumberOfDaysOutsideCoun";
//                }
//                if (i == 3)
//                {
//                    AsyncAge asyncAge = new AsyncAge() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
//                    _context.AsyncAgeDBS.Add(asyncAge);
//                    _context.SaveChanges();
//                    rabbitMQobj.ProcID = asyncAge.ID;
//                    rabbitMQobj.URL = "https://host.docker.internal:40018/RecordAdminstration/GetAge";
//                    properties.CorrelationId = "GetAge";
//                }

//                var mess = JsonSerializer.Serialize(rabbitMQobj);
//                var body = Encoding.UTF8.GetBytes(mess);

//                channel.BasicPublish("", "requestQueue", properties, body);
//            }
//            System.Console.ReadLine();

//        }

//        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId, int processID)
//        {
//            switch (correlationId)
//            {
//                case "GetIstravel":

//                    Asynctravel asynctravel = _context.AsynctravelDBS.Find(externalAPIResponce.ProcID);

//                    asynctravel.travel = true;// bool.Parse(externalAPIResponce.Responce);
//                    asynctravel.RequestReciveTime = DateTime.Now;
//                    _context.AsynctravelDBS.Update(asynctravel);
//                    _context.SaveChanges();

//                    break;
//                case "GetUserTransactions":

//                    AsyncUserTransactions asyncUserTransactions = _context.AsyncUserTransactionsDBS.Find(externalAPIResponce.ProcID);

//                    asyncUserTransactions.UserTransactions = true;// bool.Parse(externalAPIResponce.Responce);
//                    asyncUserTransactions.RequestReciveTime = DateTime.Now;
//                    _context.AsyncUserTransactionsDBS.Update(asyncUserTransactions);
//                    _context.SaveChanges();

//                    break;
//                case "GetNumberOfDaysOutsideCoun":

//                    AsyncDaysOutsideCoun asyncDaysOutsideCoun=_context.AsyncDaysOutsideCounDBS.Find(externalAPIResponce.ProcID);

//                    asyncDaysOutsideCoun.DaysOutsideCoun = 1;// Int32.Parse(externalAPIResponce.Responce);
//                    asyncDaysOutsideCoun.RequestReciveTime = DateTime.Now;
//                    _context.AsyncDaysOutsideCounDBS.Update(asyncDaysOutsideCoun);
//                    _context.SaveChanges();

//                    break;
//                case "GetAge":

//                    AsyncAge asyncAge = _context.AsyncAgeDBS.Find(externalAPIResponce.ProcID);
//                    asyncAge.Age = 1;// Int32.Parse(externalAPIResponce.Responce);
//                    asyncAge.RequestReciveTime = DateTime.Now;
//                    _context.AsyncAgeDBS.Update(asyncAge);
//                    _context.SaveChanges();

//                    break;
//            }
//            CheckIfFinish(processID);
//        }

//        private void CheckIfFinish(int procID)
//        {
//            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
//            Asynctravel asynctravel = _context.AsynctravelDBS.Where(x => x.RequestStatuesID== requestStatues).FirstOrDefault();
//            AsyncUserTransactions asyncUserTransactions = _context.AsyncUserTransactionsDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
//            AsyncDaysOutsideCoun asyncDaysOutsideCoun = _context.AsyncDaysOutsideCounDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
//            AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

//            //in gov process request canceled after certain time
//            int NumberOfDaystoAllow = -15;

//            if (asynctravel.RequestReciveTime>DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncUserTransactions.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncDaysOutsideCoun.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) )
//            {
//                requestStatues.DateOfDone = DateTime.Now;
//                requestStatues.Statues = "Done";
//                _context.RequestStatuesDBS.Update(requestStatues);
//                _context.SaveChanges();

//                if (asyncAge.Age <= 42)
//                {
//                    if (asynctravel.travel)
//                    {
//                        if (asyncUserTransactions.UserTransactions)
//                        {
//EndOtherPostponment(requestStatues.UserID);

//                            AddCert(requestStatues.UserID);

//                        }
//                    }
//                }
//            }
//else
//{
//    requestStatues.DateOfDone = DateTime.Now;
//    requestStatues.Statues = "Faild";
//    _context.RequestStatuesDBS.Update(requestStatues);
//    _context.SaveChanges();
//}
//        }

//        private void EndOtherPostponment(int UserID)
//{
//    var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
//    using (var connection = factory.CreateConnection())
//    using (var channel = connection.CreateModel())
//    {
//        channel.ExchangeDeclare(exchange: "EndActiveCert", type: ExchangeType.Fanout);

//        var message = UserID;
//        var body = Encoding.UTF8.GetBytes(message.ToString());
//        channel.BasicPublish(exchange: "EndActiveCert", routingKey: "", basicProperties: null, body: body);

//    }
//}

//        private void AddCert(int CUserID)
//        {
//            CashAllowancLessThan42Model tra = new CashAllowancLessThan42Model { UserID = CUserID, DateOfGiven = DateTime.Now };
//            _context.CashAllowancLessThan42Db.Add(tra);
//            _context.SaveChanges();

//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            return null;
//        }
//    }
//}
