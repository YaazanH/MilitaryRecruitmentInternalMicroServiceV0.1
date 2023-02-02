using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TravelApprovalAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TravelApprovalAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TravelApprovalAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly TravelApprovalContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        string ExternalIP = "192.168.168.116";

        public RabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<TravelApprovalContext>();
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

            var queName = channel.QueueDeclare(queue:"Travel",durable: true,autoDelete: false,exclusive: false,arguments: null).QueueName;
            //var queName = channel.QueueDeclare(/*queue: "travel"*/).QueueName;

                channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "TravelApproval");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {

                    var recbody = ea.Body.ToArray();

                    var recmess = Encoding.UTF8.GetString(recbody);

                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                    var User = _context.TravelApprovalDb.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
                    if (User == null)
                    {

                            int ReqStatuesID = InsertRequestToDB(userInfo.UserID);
                            SendToExternalAPI(userInfo.JWT, ReqStatuesID);
                        
                    } 
                    else
                    {
                        if (User.DateOfEnd.DateTime > DateTime.Now)
                        {
                            int ReqStatuesID = InsertRequestToDB(userInfo.UserID);
                            SendToExternalAPI(userInfo.JWT, ReqStatuesID);
                        }
                    }
                   //channel.BasicAck(deliveryTag:ea.DeliveryTag,multiple:true);
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
            rs.PostponmentType = "TravelApproval";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }



        public void SendToExternalAPI(String Token, int ReqStatuesID)
        {

           /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();*/


            var replyQueue = channel.QueueDeclare(queue: "Travelreply", durable: true, autoDelete: false, exclusive: false, arguments: null);

            

            channel.QueueDeclare(queue: "requestQueue", exclusive: false, durable: true, autoDelete: false, arguments: null);

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
            properties.Persistent=true;
            properties.ReplyTo = replyQueue.QueueName;
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(ReqStatuesID);
            RabbitMQobj rabbitMQobj = new RabbitMQobj() { JWT = Token };
            for (int i = 0; i < 3; i++)
            {
                if (i == 2)
                {
                    Asynctravel asynctravel = new Asynctravel() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now ,statuse="wating"};
                    _context.AsynctravelDBS.Add(asynctravel);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://"+ExternalIP+":40011/Passport/GetIstravel";
                    properties.CorrelationId = "GetIstravel";
                    rabbitMQobj.ProcID = asynctravel.ID;
                }

                else if (i == 1)
                {
                    AsynLabor asynLabor = new AsynLabor() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, statuse = "wating" };
                    _context.AsynLaborDBS.Add(asynLabor);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asynLabor.ID;
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40024/LaborMinAPI/GetIsAWorker";
                    properties.CorrelationId = "GetasynLabor";
                }
                else if(i == 0)
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
                properties.Persistent = true;

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId, int processID)
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

                case "GetasynLabor":

                    AsynLabor asynLabor = _context.AsynLaborDBS.Find(externalAPIResponce.ProcID);

                    asynLabor.IsALaborWorker =  bool.Parse(externalAPIResponce.Responce);
                    asynLabor.RequestReciveTime = DateTime.Now;
                    asynLabor.statuse = "Done";
                    _context.AsynLaborDBS.Update(asynLabor);
                    _context.SaveChanges();

                    break;
                case "GetAge":

                    AsyncAge asyncAge = _context.AsyncAgeDBS.Find(externalAPIResponce.ProcID);
                    asyncAge.Age =  Int32.Parse(externalAPIResponce.Responce);
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
            AsynLabor asynLabor = _context.AsynLaborDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;

            if (asyncAge.statuse=="Done" && asynLabor.statuse == "Done" && asynctravel.statuse == "Done")
            {
                if (asynctravel.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asynLabor.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
                {
                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();
                    if (asyncAge.Age <= 42)
                    {
                        if (!asynLabor.IsALaborWorker)
                        {

                            if (asynctravel.travel)
                            {
                                EndOtherPostponment(requestStatues.UserID);
                                AddCert(requestStatues.UserID);
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
        private void EndOtherPostponment(int UserID)
        {
            /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
             using (var connection = factory.CreateConnection())
             using (var channel = connection.CreateModel())*/

            channel.ExchangeDeclare(exchange: "EndActiveCert", type: ExchangeType.Fanout);

            var message = UserID;
            var body = Encoding.UTF8.GetBytes(message.ToString());
            var prop = channel.CreateBasicProperties();
            prop.Persistent = true;
            channel.BasicPublish(exchange: "EndActiveCert", routingKey: "", prop, body: body);


        }

        private void AddCert(int CUserID)
        {
            TravelApproval tra = new TravelApproval { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
            _context.TravelApprovalDb.Add(tra);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
