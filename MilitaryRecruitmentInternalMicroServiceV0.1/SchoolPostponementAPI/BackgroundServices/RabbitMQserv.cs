﻿using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolPostponementAPI.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using SchoolPostponementAPI.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SchoolPostponementAPI.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly SchoolPostponementContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        string ExternalIP = "192.168.168.116";


        public RabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<SchoolPostponementContext>();
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

            var queName = channel.QueueDeclare(queue: "school", durable: true, autoDelete: false, exclusive: false, arguments: null).QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "SchoolPostponement");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {

                    var recbody = ea.Body.ToArray();

                    var recmess = Encoding.UTF8.GetString(recbody);

                    UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                    var User = _context.schoolDBS.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
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
            rs.PostponmentType = "SchoolPostponement";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }



        public void SendToExternalAPI(String Token, int ReqStatuesID)
        {

            /* var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

             using var connection = factory.CreateConnection();

             using var channel = connection.CreateModel();*/

            var replyQueue= channel.QueueDeclare(queue: "schoolreply", durable: true, autoDelete: false, exclusive: false, arguments: null);


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
                    AsyncStudyYears asyncStudyYears = new AsyncStudyYears() { RequestStatuesID = requestStatues,RequestSendTime = DateTime.Now ,statuse ="wating" };
                    _context.AsyncStudyYearsDBS.Add(asyncStudyYears);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40006/University/GetStudyYears";
                    properties.CorrelationId = "GetStudyYears";
                    rabbitMQobj.ProcID = asyncStudyYears.ID;
                }

                if (i == 1)
                {
                    AsyncStudyingNow asyncStudyingNow = new AsyncStudyingNow() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, statuse = "wating" };
                    _context.AsyncStudyingNowDBS.Add(asyncStudyingNow);
                    _context.SaveChanges();
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40006/University/GetIsStudyingNow";
                    properties.CorrelationId = "GetStudyingNow";
                    rabbitMQobj.ProcID = asyncStudyingNow.ID;
                }
                if (i == 2)
                {
                    AsyncDroppedOut asyncDroppedOut = new AsyncDroppedOut() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, statuse = "wating" };
                    _context.AsyncDroppedOutDBS.Add(asyncDroppedOut);
                    _context.SaveChanges();
                    rabbitMQobj.ProcID = asyncDroppedOut.ID;
                    rabbitMQobj.URL = "https://" + ExternalIP + ":40006/EduMinAPI/GetIsDroppedOut";
                    properties.CorrelationId = "GetIsDroppedOut";
                }
                if (i == 3)
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

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            switch (correlationId)
            {
                case "GetIsDroppedOut":

                    AsyncDroppedOut asyncDroppedOut = _context.AsyncDroppedOutDBS.Find(externalAPIResponce.ProcID);

                    asyncDroppedOut.IsDroppedOut = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncDroppedOut.RequestReciveTime = DateTime.Now;
                    asyncDroppedOut.statuse = "Done";
                    _context.AsyncDroppedOutDBS.Update(asyncDroppedOut);
                    _context.SaveChanges();

                    break;
                case "GetStudyingNow":

                    AsyncStudyingNow asyncStudyingNow = _context.AsyncStudyingNowDBS.Find(externalAPIResponce.ProcID);

                    asyncStudyingNow.StudyingNow = true;// bool.Parse(externalAPIResponce.Responce);
                    asyncStudyingNow.RequestReciveTime = DateTime.Now;
                    asyncStudyingNow.statuse = "Done";
                    _context.AsyncStudyingNowDBS.Update(asyncStudyingNow);
                    _context.SaveChanges();

                    break;
                case "GetStudyYears":

                    AsyncStudyYears asyncStudyYears = _context.AsyncStudyYearsDBS.Find(externalAPIResponce.ProcID);

                    asyncStudyYears.StudyYears = "1";//  externalAPIResponce.Responce.ToString();
                    asyncStudyYears.RequestReciveTime = DateTime.Now;
                    asyncStudyYears.statuse = "Done";
                    _context.AsyncStudyYearsDBS.Update(asyncStudyYears);
                    _context.SaveChanges();

                    break;
                case "GetAge":

                    AsyncAge asyncAge = _context.AsyncAgeDBS.Find(externalAPIResponce.ProcID);
                    asyncAge.Age = 1;// Int32.Parse(externalAPIResponce.Responce);
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
            AsyncStudyingNow asyncStudyingNow = _context.AsyncStudyingNowDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncDroppedOut asyncDroppedOut = _context.AsyncDroppedOutDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncStudyYears asyncStudyYears = _context.AsyncStudyYearsDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

            //in gov process request canceled after certain time
            int NumberOfDaystoAllow = -15;
            if (asyncAge.statuse == "Done" && asyncStudyingNow.statuse == "Done" && asyncDroppedOut.statuse == "Done"&& asyncStudyYears.statuse=="Done")
            {
                if (asyncStudyingNow.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncDroppedOut.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncStudyYears.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow) && asyncAge.RequestReciveTime > DateTime.Today.AddDays(NumberOfDaystoAllow))
                {
                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();

                    if (asyncAge.Age < 37)
                    {
                        if (asyncStudyingNow.StudyingNow)
                        {
                            if (!asyncDroppedOut.IsDroppedOut)
                            {
                                switch (asyncStudyYears.StudyYears)
                                {
                                    case "2":
                                        if (asyncAge.Age < 24)
                                        {
                                            EndOtherPostponment(requestStatues.UserID);

                                            AddCert(requestStatues.UserID);

                                        }
                                        break;
                                    case "3":
                                        if (asyncAge.Age < 25)
                                        {
                                            EndOtherPostponment(requestStatues.UserID);

                                            AddCert(requestStatues.UserID);

                                        }
                                        break;
                                    case "4":
                                        if (asyncAge.Age < 26)
                                        {
                                            EndOtherPostponment(requestStatues.UserID);

                                            AddCert(requestStatues.UserID);

                                        }
                                        break;
                                    case "5":
                                        if (asyncAge.Age < 27)
                                        {
                                            EndOtherPostponment(requestStatues.UserID);

                                            AddCert(requestStatues.UserID);

                                        }
                                        break;
                                    case "6":
                                        if (asyncAge.Age < 29)
                                        {
                                            EndOtherPostponment(requestStatues.UserID);

                                            AddCert(requestStatues.UserID);

                                        }
                                        break;
                                    default:
                                        PostponmentFalid(requestStatues);
                                        break;
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
            SchoolPostponement st = new SchoolPostponement { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddYears(1) };
            _context.schoolDBS.Add(st);
            _context.SaveChanges();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
