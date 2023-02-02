using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AlonePostponement.Data;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using AlonePostponement.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace AlonePostponement.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        private readonly AlonePostponementContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        string ExternalIP = "192.168.168.116";

        public RabbitMQserv(IServiceScopeFactory Ifactory)
        {
            _context = Ifactory.CreateScope().ServiceProvider.GetRequiredService<AlonePostponementContext>();

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

            var queName = channel.QueueDeclare(queue: "Alone", durable: true, autoDelete: false, exclusive: false, arguments: null).QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "AlonePostponement");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                var User = _context.AlonePostponementDBS.Where(x => x.UserID == userInfo.UserID).FirstOrDefault();
                if (User == null)
                {

                    int ReqStatuesID = InsertRequestToDB(userInfo);
                    SendToExternalAPI(userInfo.JWT, ReqStatuesID);

                }
                else
                {
                    if (User.DateOfEnd.DateTime > DateTime.Now)
                    {
                        int ReqStatuesID = InsertRequestToDB(userInfo);
                        SendToExternalAPI(userInfo.JWT, ReqStatuesID);
                    }
                }
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
            System.Console.Read();
            return null;

        }


        private int InsertRequestToDB(UserInfo user)
        {
            RequestStatues rs = new RequestStatues();
            rs.UserID = user.UserID;
            rs.DateOfRecive = DateTime.Now;
            rs.UserJWT = user.JWT;
            rs.Statues = "wating";
            rs.PostponmentType = "AlonePostponement";
            _context.RequestStatuesDBS.Add(rs);
            _context.SaveChanges();

            return rs.ReqStatuesID;
        }

        public void SendToExternalAPI(String Token, int processID)
        {

            /*var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();*/


            var replyQueue = channel.QueueDeclare(queue: "AloneReply", durable: true, autoDelete: false, exclusive: false, arguments: null);

            

            channel.QueueDeclare(queue: "requestQueue", durable: true, autoDelete: false, exclusive: false, arguments: null );

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
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(processID);

            BrothersID brothersID = _context.BrothersIDDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
            var properties = channel.CreateBasicProperties();
            RabbitMQobj rabbitMQobj = new RabbitMQobj() { ProcID = processID, JWT = Token };
            properties.ReplyTo = replyQueue.QueueName;

            if (brothersID!=null&&brothersID.Statues == "Done")
            {
                for (int j = 0; j < brothersID.BrotherID.Split(',').ToList().Count; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {



                        if (i == 0)
                        {
                            BrotherEill brotherEill = new BrotherEill() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
                            _context.BrotherEillDBS.Add(brotherEill);
                            _context.SaveChanges();

                            rabbitMQobj.ProcID = brotherEill.ID;
                            rabbitMQobj.URL = "https://" + ExternalIP + ":40003/HealthMin/GetHaveProb" + (int)brothersID.BrotherID[j];

                            properties.CorrelationId = "AlonepostmentBrothersEill";
                        }
                        if (i == 1)
                        {
                            DeadBrothers deadBrothers = new DeadBrothers() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
                            _context.DeadBrothersDBS.Add(deadBrothers);
                            _context.SaveChanges();

                            rabbitMQobj.ProcID = deadBrothers.ID;
                            rabbitMQobj.URL = "https://" + ExternalIP + ":40018/RecordAdminstration/GetDeath" + (int)brothersID.BrotherID[j]; ;
                            properties.CorrelationId = "AlonepostmentDeadBrothers";
                        }



                    }
                }
            }
            else
            {
                BrothersID SbrothersID = new BrothersID() { RequestStatuesID = requestStatues, RequestSendTime = DateTime.Now, Statues = "wating" };
                _context.BrothersIDDBS.Add(SbrothersID);
                _context.SaveChanges();

                rabbitMQobj.ProcID = SbrothersID.ID;
                rabbitMQobj.URL = "https://" + ExternalIP + ":40018/RecordAdminstration/GetListOfBros";
                properties.CorrelationId = "AlonepostmentBrothersID";
            }





            var mess = JsonSerializer.Serialize(rabbitMQobj);
            var body = Encoding.UTF8.GetBytes(mess);
            properties.Persistent=true;
            channel.BasicPublish("", "requestQueue", properties, body);

            System.Console.ReadLine();

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(externalAPIResponce.ProcID);

            switch (correlationId)
            {

                case "AlonepostmentBrothersID":

                    BrothersID brothersID = _context.BrothersIDDBS.Find(externalAPIResponce.ProcID);
                    brothersID.BrotherID =  externalAPIResponce.Responce;
                    brothersID.RequestReciveTime = DateTime.Now;
                    brothersID.Statues = "Done";
                    _context.BrothersIDDBS.Update(brothersID);
                    _context.SaveChanges();
                    if (brothersID.BrotherID.Split(',').ToList().Count()>0)
                    {
                        SendToExternalAPI(requestStatues.UserJWT, externalAPIResponce.RequestStatuseID);
                    }
                    
                    break;

                case "AlonepostmentBrothersEill":
                    BrotherEill brotherEill = _context.BrotherEillDBS.Find(externalAPIResponce.ProcID);
                    brotherEill.AllBrotherEill =bool.Parse( externalAPIResponce.Responce);
                    brotherEill.RequestReciveTime = DateTime.Now;
                    brotherEill.Statues = "Done";
                    _context.BrotherEillDBS.Update(brotherEill);
                    _context.SaveChanges();

                    break;

                case "AlonepostmentDeadBrothers":

                    DeadBrothers deadBrothers = _context.DeadBrothersDBS.Find(externalAPIResponce.ProcID);
                    deadBrothers.AllDeadBrothers = bool.Parse(externalAPIResponce.Responce);
                    deadBrothers.RequestReciveTime = DateTime.Now;
                    deadBrothers.Statues = "Done";
                    _context.DeadBrothersDBS.Update(deadBrothers);
                    _context.SaveChanges();

                    break;
            }
            CheckIfFinish(externalAPIResponce.RequestStatuseID);
        }

        private void CheckIfFinish(int procID)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(procID);
            List<BrotherEill> brotherEill = _context.BrotherEillDBS.Where(x => x.RequestStatuesID == requestStatues&&x.Statues=="Done").ToList();
            BrothersID brothersID = _context.BrothersIDDBS.Where(x => x.RequestStatuesID == requestStatues && x.Statues == "Done").FirstOrDefault();
            List<DeadBrothers> deadBrothers = _context.DeadBrothersDBS.Where(x => x.RequestStatuesID == requestStatues && x.Statues == "Done").ToList();

            int brotherscount = brothersID.BrotherID.Split(',').ToList().Count();

            if (brotherscount == 0)
            {
                AddCert(requestStatues.UserID);
            }
            else if (brotherEill.Count() == brotherscount && deadBrothers.Count() == brotherscount)
            {

                //in gov process request canceled after certain time
                int NumberOfDaystoAllow = -15;
                bool Timecheck = true;

                for (int i = 0; i < brotherscount; i++)
                {
                    if (brotherEill[i].RequestReciveTime < DateTime.Today.AddDays(NumberOfDaystoAllow) || deadBrothers[i].RequestReciveTime < DateTime.Today.AddDays(NumberOfDaystoAllow))
                    {
                        Timecheck = false;

                        break;
                    }
                }

                if (Timecheck)
                {
                    requestStatues.DateOfDone = DateTime.Now;
                    requestStatues.Statues = "Done";
                    _context.RequestStatuesDBS.Update(requestStatues);
                    _context.SaveChanges();
                    List<RequestStatues> BrotherNotDied = new List<RequestStatues>();
                    List<RequestStatues> BrotherNoteill = new List<RequestStatues>();

                    foreach (var item in deadBrothers)
                    {
                        //if any brother death
                        if (!item.AllDeadBrothers)
                        {
                            BrotherNotDied.Add(item.RequestStatuesID);
                        }
                    }
                    if (BrotherNotDied.Count > 0)
                    {
                        foreach (var item in BrotherNotDied)
                        {
                            BrotherEill brotherEill1 = brotherEill.Where(x => x.RequestStatuesID == item).FirstOrDefault();
                            //check if one of the brother not eill
                            if (!brotherEill1.AllBrotherEill)
                            {
                                BrotherNoteill.Add(item);
                            }
                        }
                    }

                    if (BrotherNoteill.Count<1)
                    {
                        EndOtherPostponment(requestStatues.UserID);
                        AddCert(requestStatues.UserID);
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




        }

        private void AddCert(int CUserID)
        {
            Models.AlonePostponement tra = new Models.AlonePostponement { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(36) };
            _context.AlonePostponementDBS.Add(tra);
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


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
