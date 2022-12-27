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

namespace CashAllowancLessThan42.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly PostponementOfConvictsContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQserv(IServiceScopeFactory factory)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<PostponementOfConvictsContext>();
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
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    AsyncInJail asyncInJail = new AsyncInJail() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
                    _context.AsyncInJailDb.Add(asyncInJail);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://host.docker.internal:40015/Jail/GetIfInJail";
                    properties.CorrelationId = "GetInJail";
                    rabbitMQobj.ProcID = asyncInJail.ID;
                }

                if (i == 1)
                {
                    AsyncYearsRemaning asyncYearsRemaning = new AsyncYearsRemaning() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
                    _context.AsyncYearsRemaningDb.Add(asyncYearsRemaning);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://host.docker.internal:40012/Court/GetYearsRemaining";
                    properties.CorrelationId = "GetYearsRemaning";
                    rabbitMQobj.ProcID = asyncYearsRemaning.ID;
                }
                if (i == 2)
                {
                    AsyncEntryDate asyncEntryDate = new AsyncEntryDate() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now };
                    _context.AsyncEntryDateDb.Add(asyncEntryDate);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asyncEntryDate.ID;
                    rabbitMQobj.URL = "https://host.docker.internal:40016/CompetentAuthority/GetEntryDate";
                    properties.CorrelationId = "GetEntryDate";
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
                case "GetInJail":

                    AsyncInJail asyncInJail = _context.AsyncInJailDb.Find(externalAPIResponce.ProcID);

                    asyncInJail.InJail = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncInJail.RequestReciveTime = DateTime.Now;
                    _context.AsyncInJailDb.Update(asyncInJail);
                    _context.SaveChanges();

                    break;
                case "GetYearsRemaning":

                    AsyncYearsRemaning asyncYearsRemaning = _context.AsyncYearsRemaningDb.Find(externalAPIResponce.ProcID);

                    asyncYearsRemaning.Years = 5;// bool.Parse(externalAPIResponce.Responce);
                    asyncYearsRemaning.RequestReciveTime = DateTime.Now;
                    _context.AsyncYearsRemaningDb.Update(asyncYearsRemaning);
                    _context.SaveChanges();

                    break;
                case "GetEntryDate":

                    AsyncEntryDate asyncEntryDate = _context.AsyncEntryDateDb.Find(externalAPIResponce.ProcID);

                    asyncEntryDate.Entrydate = DateTime.Now;// Int32.Parse(externalAPIResponce.Responce);
                    asyncEntryDate.RequestReciveTime = DateTime.Now;
                    _context.AsyncEntryDateDb.Update(asyncEntryDate);
                    _context.SaveChanges();

                    break;

            }
            CheckIfFinish(processID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            AsyncInJail asyncInJail = _context.AsyncInJailDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncEntryDate asyncEntryDate = _context.AsyncEntryDateDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncYearsRemaning asyncYearsRemaning = _context.AsyncYearsRemaningDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;


            if (asyncEntryDate.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncYearsRemaning.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncInJail.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
            {
                requestStatues.DateOfDone = DateTime.Now;
                requestStatues.Statues = "Done";
                _context.RequestStatuesDBS.Update(requestStatues);
                _context.SaveChanges();

                if (asyncEntryDate.Entrydate == DateTime.Now || asyncInJail.InJail || asyncYearsRemaning.Years == 5)
                {



                    AddCert(requestStatues.UserID);



                }
            }
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
