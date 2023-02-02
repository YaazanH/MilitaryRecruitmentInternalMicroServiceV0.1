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

        string ExternalIP = "192.168.168.116";

        public RabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<CashAllowanceContext>();
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

            var queName = channel.QueueDeclare(queue: "CashAllow", durable: true, autoDelete: false, exclusive: false, arguments: null).QueueName;


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

            /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

             using var connection = factory.CreateConnection();

             using var channel = connection.CreateModel();*/

            var replyQueue = channel.QueueDeclare(queue: "cashreply", durable: true, autoDelete: false, exclusive: false, arguments: null);


            channel.QueueDeclare(queue: "requestQueue",durable: true, autoDelete: false, exclusive: false, arguments: null );

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

                    AsyncAge asyncage = new AsyncAge() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now,Statues="wating" };
                    _context.AsyncAgeDb.Add(asyncage);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40018/RecordAdminstration/GetAge";
                    properties.CorrelationId = "GetAge";
                    rabbitMQobj.ProcID = asyncage.ID;
                


             
                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);
            properties.Persistent = true;

            channel.BasicPublish("", "requestQueue", properties, body);
            
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            switch (correlationId)
            {
       
                case "GetAge":

                    AsyncAge asyncAge = _context.AsyncAgeDb.Find(externalAPIResponce.ProcID);
                    asyncAge.Age = 1;// Int32.Parse(externalAPIResponce.Responce);
                    asyncAge.RequestReciveTime = DateTime.Now;
                    asyncAge.Statues = "Done";
                    _context.AsyncAgeDb.Update(asyncAge);
                    _context.SaveChanges();

                    break;
            }
            CheckIfFinish(externalAPIResponce.RequestStatuseID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);                   
           
            AsyncAge asyncAge = _context.AsyncAgeDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;
            if (asyncAge.Statues == "Done")
            {

                if (asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
                {

                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();
                    if (asyncAge.Age >= 42)
                    {
                        string typetopay = "";
                        switch (asyncAge.Age)
                        {
                            case 42:
                                typetopay = "CashAllowance42";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 43:
                                typetopay = "CashAllowance43";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 44:
                                typetopay = "CashAllowance44";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 45:
                                typetopay = "CashAllowance45";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 46:
                                typetopay = "CashAllowance46";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 47:
                                typetopay = "CashAllowance47";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 48:
                                typetopay = "CashAllowance48";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 49:
                                typetopay = "CashAllowance49";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 50:
                                typetopay = "CashAllowance50";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            case 51:
                                typetopay = "CashAllowance51";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;
                            default:
                                typetopay = "CashAllowance52";
                                CalculateExtraPAyment(requestStatues, typetopay);
                                break;

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
        }
        public void CalculateExtraPAyment(RequestStatues requestStatues,string Name)
        {
            /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

             using var connection = factory.CreateConnection();

             using var channel = connection.CreateModel();*/

            var replyQueue = channel.QueueDeclare(queue: "cashallopay", durable: true, autoDelete: false, exclusive: false, arguments: null);

            channel.QueueDeclare(queue: "UserCalcPayment", durable: true, autoDelete: false, exclusive: false, arguments: null);

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
            properties.Persistent = true;

            channel.BasicPublish("", "UserCalcPayment", properties, body);

            System.Console.ReadLine();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
