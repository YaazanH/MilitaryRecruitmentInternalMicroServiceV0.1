using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostponementOfConvictsAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using PostponementOfConvictsAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace PostponementOfConvictsAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly PostponementOfConvictsContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        string ExternalIP = "192.168.168.116";


        public RabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<PostponementOfConvictsContext>();
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

            var queName = channel.QueueDeclare(queue: "convicts", durable: true, autoDelete: false, exclusive: false, arguments: null).QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "PostponementOfConvicts");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                var User = _context.PostponementOfConvictsDb.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
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
            rs.PostponmentType = "PostponementOfConvicts";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }



        public void SendToExternalAPI(String Token, int ReqStatuesID)
        {

            /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

             using var connection = factory.CreateConnection();

             using var channel = connection.CreateModel();*/
            var replyQueue= channel.QueueDeclare(queue: "convictsreply", durable: true, autoDelete: false, exclusive: false, arguments: null);

            channel.QueueDeclare(queue: "requestQueue",durable: true, autoDelete: false, exclusive: false, arguments: null);

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
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    AsyncInJail asyncInJail = new AsyncInJail() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now,Statues="wating" };
                    _context.AsyncInJailDb.Add(asyncInJail);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40015/Jail/GetIfInJail";
                    properties.CorrelationId = "GetInJail";
                    rabbitMQobj.ProcID = asyncInJail.ID;
                }

                if (i == 1)
                {
                    AsyncYearsRemaning asyncYearsRemaning = new AsyncYearsRemaning() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
                    _context.AsyncYearsRemaningDb.Add(asyncYearsRemaning);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40012/Court/GetYearsRemaining";
                    properties.CorrelationId = "GetYearsRemaning";
                    rabbitMQobj.ProcID = asyncYearsRemaning.ID;
                }
                if (i == 2)
                {
                    AsyncEntryDate asyncEntryDate = new AsyncEntryDate() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
                    _context.AsyncEntryDateDb.Add(asyncEntryDate);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asyncEntryDate.ID;
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40016/CompetentAuthority/GetEntryDate";
                    properties.CorrelationId = "GetEntryDate";
                }


                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);
                properties.Persistent = true;

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            switch (correlationId)
            {
                case "GetInJail":

                    AsyncInJail asyncInJail = _context.AsyncInJailDb.Find(externalAPIResponce.ProcID);

                    asyncInJail.InJail = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncInJail.RequestReciveTime = DateTime.Now;
                    asyncInJail.Statues = "Done";
                    _context.AsyncInJailDb.Update(asyncInJail);
                    _context.SaveChanges();

                    break;
                case "GetYearsRemaning":

                    AsyncYearsRemaning asyncYearsRemaning = _context.AsyncYearsRemaningDb.Find(externalAPIResponce.ProcID);

                    asyncYearsRemaning.Years = 5;// bool.Parse(externalAPIResponce.Responce);
                    asyncYearsRemaning.RequestReciveTime = DateTime.Now;
                    asyncYearsRemaning.Statues = "Done";
                    _context.AsyncYearsRemaningDb.Update(asyncYearsRemaning);
                    _context.SaveChanges();

                    break;
                case "GetEntryDate":

                    AsyncEntryDate asyncEntryDate = _context.AsyncEntryDateDb.Find(externalAPIResponce.ProcID);

                    asyncEntryDate.Entrydate = DateTime.Now;// Int32.Parse(externalAPIResponce.Responce);
                    asyncEntryDate.RequestReciveTime = DateTime.Now;
                    asyncEntryDate.Statues = "Done";
                    _context.AsyncEntryDateDb.Update(asyncEntryDate);
                    _context.SaveChanges();

                    break;

            }
            CheckIfFinish(externalAPIResponce.RequestStatuseID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            AsyncInJail asyncInJail = _context.AsyncInJailDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncEntryDate asyncEntryDate = _context.AsyncEntryDateDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncYearsRemaning asyncYearsRemaning = _context.AsyncYearsRemaningDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;  

            if (asyncInJail.Statues == "Done" && asyncEntryDate.Statues == "Done" && asyncYearsRemaning.Statues == "Done")
            {
                if (asyncEntryDate.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncYearsRemaning.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncInJail.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
                {
                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();

                    if (asyncEntryDate.Entrydate == DateTime.Now || asyncInJail.InJail || asyncYearsRemaning.Years == 5)
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
            /*var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
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
            PostponementOfConvicts tra = new PostponementOfConvicts { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
            _context.PostponementOfConvictsDb.Add(tra);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
