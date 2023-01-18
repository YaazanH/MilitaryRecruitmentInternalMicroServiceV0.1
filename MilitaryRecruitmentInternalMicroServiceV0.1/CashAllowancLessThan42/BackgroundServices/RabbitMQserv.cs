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
    public class RabbitMQserv : BackgroundService
    {
        private readonly CashAllowancLessThan42Context _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        string ExternalIP = "192.168.168.116";

        public RabbitMQserv(IServiceScopeFactory Ifactory)
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

                channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

                var queName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "CashAllowancLessThan42");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {

                    var recbody = ea.Body.ToArray();

                    var recmess = Encoding.UTF8.GetString(recbody);

                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                    var User = _context.CashAllowancLessThan42Db.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
                   //have to change to ==  bec != for test
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
            rs.PostponmentType = "CashAllowancLessThan42";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }



        public void SendToExternalAPI(String Token, int ReqStatuesID)
        {

            /*var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();*/


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
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(ReqStatuesID);
            RabbitMQobj rabbitMQobj = new RabbitMQobj() { JWT = Token };
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    Asynctravel asynctravel = new Asynctravel() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now,statuse="wating" };
                    _context.AsynctravelDBS.Add(asynctravel);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40011/Passport/GetIstravel";
                    properties.CorrelationId = "GetIstravel";
                    rabbitMQobj.ProcID = asynctravel.ID;
                }

                else if (i == 1)
                {
                    AsyncDaysOutsideCoun asyncDaysOutsideCoun = new AsyncDaysOutsideCoun() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, statuse = "wating" };
                    _context.AsyncDaysOutsideCounDBS.Add(asyncDaysOutsideCoun);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asyncDaysOutsideCoun.ID;
                    rabbitMQobj.URL =  "https://" + ExternalIP + ":40011/Passport/GetNumberOfDaysOutsideCoun";
                    properties.CorrelationId = "GetNumberOfDaysOutsideCoun";
                }
                else if (i == 2)
                {
                    AsyncAge asyncAge = new AsyncAge() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, statuse = "wating" };
                    _context.AsyncAgeDBS.Add(asyncAge);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asyncAge.ID;
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40018/RecordAdminstration/GetAge";
                    properties.CorrelationId = "GetAge";
                }

                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            switch (correlationId)
            {
                case "GetIstravel":

                    Asynctravel asynctravel = _context.AsynctravelDBS.Find(externalAPIResponce.ProcID);

                    asynctravel.travel =  bool.Parse(externalAPIResponce.Responce);
                    asynctravel.RequestReciveTime = DateTime.Now;
                    asynctravel.statuse = "Done";
                    _context.AsynctravelDBS.Update(asynctravel);
                    _context.SaveChanges();

                    break;
                case "GetNumberOfDaysOutsideCoun":

                    AsyncDaysOutsideCoun asyncDaysOutsideCoun=_context.AsyncDaysOutsideCounDBS.Find(externalAPIResponce.ProcID);

                    asyncDaysOutsideCoun.DaysOutsideCoun = 1;// Int32.Parse(externalAPIResponce.Responce);
                    asyncDaysOutsideCoun.RequestReciveTime = DateTime.Now;
                    asyncDaysOutsideCoun.statuse = "Done";
                    _context.AsyncDaysOutsideCounDBS.Update(asyncDaysOutsideCoun);
                    _context.SaveChanges();

                    break;
                case "GetAge":

                    AsyncAge asyncAge = _context.AsyncAgeDBS.Find(externalAPIResponce.ProcID);
                    asyncAge.Age = 1;//  Int32.Parse(externalAPIResponce.Responce);
                    asyncAge.RequestReciveTime = DateTime.Now;
                    asyncAge.statuse = "Done";
                    _context.AsyncAgeDBS.Update(asyncAge);
                    _context.SaveChanges();

                    break;
            }
            CheckIfFinish(externalAPIResponce.RequestStatuseID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            Asynctravel asynctravel = _context.AsynctravelDBS.Where(x => x.RequestStatuesID== requestStatues).FirstOrDefault();          
            AsyncDaysOutsideCoun asyncDaysOutsideCoun = _context.AsyncDaysOutsideCounDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;
            if (asyncAge.statuse == "Done" && asyncDaysOutsideCoun.statuse == "Done" && asynctravel.statuse == "Done")
            {
                if (asynctravel.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncDaysOutsideCoun.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
                {
                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();

                    if (asyncAge.Age <= 42)
                    {
                        if (asynctravel.travel)
                        {

                            if (asyncDaysOutsideCoun.DaysOutsideCoun >= 1460)
                            {

                                CalculateExtraPAyment(requestStatues, "CashAllowance7");
                            }

                            else if (asyncDaysOutsideCoun.DaysOutsideCoun > 1095 && asyncDaysOutsideCoun.DaysOutsideCoun < 1460)
                            {

                                CalculateExtraPAyment(requestStatues, "CashAllowance8");
                            }

                            else if (asyncDaysOutsideCoun.DaysOutsideCoun > 730 && asyncDaysOutsideCoun.DaysOutsideCoun < 1095)
                            {

                                CalculateExtraPAyment(requestStatues, "CashAllowance9");
                            }

                            else if (asyncDaysOutsideCoun.DaysOutsideCoun > 365 && asyncDaysOutsideCoun.DaysOutsideCoun < 730)
                            {
                                CalculateExtraPAyment(requestStatues, "CashAllowance10");
                            }
                            else
                            {
                                PostponmentFalid(requestStatues);
                            }

                        }
                        else
                        {
                            PostponmentFalid(requestStatues);
                        }
                    }
                    else
                    {
                        PostponmentFalid(requestStatues);
                    }
                }
                else
                {
                    PostponmentFalid(requestStatues);
                }
            }
        }

        public void PostponmentFalid(RequestStatues requestStatues)
        {
            requestStatues.DateOfDone = DateTime.Now;
            requestStatues.Statues = "Faild";
            _context.RequestStatuesDBS.Update(requestStatues);
            _context.SaveChanges();
        }

        private void CalculateExtraPAyment(RequestStatues requestStatues,string Name)
        {
            /*var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();*/


            var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

            channel.QueueDeclare(queue: "UserCalcPayment", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                RabbitMQResponce ExternalAPIResponce = JsonSerializer.Deserialize<RabbitMQResponce>(recmess);

                AsyncPayment asyncPayment = _context.AsyncPaymentDBS.Where(x => x.PaymentID == ExternalAPIResponce.ProcID).FirstOrDefault();
                asyncPayment.EcashURl = ExternalAPIResponce.Responce;
                _context.AsyncPaymentDBS.Update(asyncPayment);
                _context.SaveChanges();

            };

            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);


            var properties = channel.CreateBasicProperties();

            properties.ReplyTo = replyQueue.QueueName;

            RabbitMQobj rabbitMQobj = new RabbitMQobj() { RequestStatuseID = requestStatues.ReqStatuesID, URL = Name };

            AsyncPayment asyncPayment = new AsyncPayment() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
            _context.AsyncPaymentDBS.Add(asyncPayment);
            _context.SaveChanges();

            rabbitMQobj.ProcID = asyncPayment.PaymentID;

            var mess = JsonSerializer.Serialize(rabbitMQobj);
            var body = Encoding.UTF8.GetBytes(mess);

            channel.BasicPublish("", "UserCalcPayment", properties, body);

            System.Console.ReadLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
