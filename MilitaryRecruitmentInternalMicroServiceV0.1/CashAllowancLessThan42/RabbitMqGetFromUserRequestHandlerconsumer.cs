using CashAllowancLessThan42.Data;
using CashAllowancLessThan42.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CashAllowancLessThan42
{

    public class RabbitMqGetFromUserRequestHandlerconsumer
    {

        private readonly CashAllowancLessThan42Context _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMqGetFromUserRequestHandlerconsumer(String Hostname, CashAllowancLessThan42Context context)
        {

            this.factory = new ConnectionFactory() { HostName = Hostname};
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "CashAllowancLessThan42");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(recmess);

                int processID= InsertRequestToDB(userInfo.UserID);
                SendToExternalAPI(userInfo.JWT,processID);
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
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

        public void SendToExternalAPI(String Token, int processID)
        {

            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            var replyQueue = channel.QueueDeclare(queue:"",exclusive:true);

            channel.QueueDeclare(queue: "requestQueue", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                 var recbody = ea.Body.ToArray();

                 var recmess = Encoding.UTF8.GetString(recbody);

                 RabbitMQResponce ExternalAPIResponce = JsonSerializer.Deserialize<RabbitMQResponce>(recmess);


                 string CorrelationId = ea.BasicProperties.CorrelationId;


                 UpdateRequestToDB(ExternalAPIResponce,CorrelationId);
            };

            channel.BasicConsume(queue:replyQueue.QueueName,autoAck:true, consumer: consumer);


            var properties = channel.CreateBasicProperties();

            properties.ReplyTo = replyQueue.QueueName;
            for (int i = 0; i < 4; i++)
            {
                RabbitMQobj rabbitMQobj= new RabbitMQobj() { ProcID=processID,JWT=Token};
                if (i==0)
                {
                    rabbitMQobj.URL = "https://host.docker.internal:40011/Passport/GetIstravel";
                    properties.CorrelationId = "GetIstravel";
                }

                if (i == 1)
                {
                    rabbitMQobj.URL = "https://host.docker.internal:40022/Finance/GetUserTransactions";
                    properties.CorrelationId = "GetUserTransactions";
                }
                if (i == 2)
                {
                    rabbitMQobj.URL = "https://host.docker.internal:40011/Passport/GetNumberOfDaysOutsideCoun";
                    properties.CorrelationId = "GetNumberOfDaysOutsideCoun";
                }
                if (i == 3)
                {
                    rabbitMQobj.URL = "https://host.docker.internal:40018/RecordAdminstration/GetAge";
                    properties.CorrelationId = "GetAge";
                }

                var mess = JsonSerializer.Serialize(rabbitMQobj);
                var body = Encoding.UTF8.GetBytes(mess);

                channel.BasicPublish("", "requestQueue", properties, body);
            }
            

        }

        private void UpdateRequestToDB(RabbitMQResponce externalAPIResponce, string correlationId)
        {
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(externalAPIResponce.ProcID);
            switch (correlationId)
            {
                case "GetIstravel":
                    
                    Asynctravel asynctravel = new Asynctravel() {RequestStatuesID=requestStatues,travel=bool.Parse(externalAPIResponce.Responce), RequestReciveTime=DateTime.Now };
                    _context.AsynctravelDBS.Add(asynctravel);
                    _context.SaveChanges();

                    break;
                case "GetUserTransactions":
                    
                    AsyncUserTransactions asyncUserTransactions = new AsyncUserTransactions() { RequestStatuesID = requestStatues, UserTransactions = bool.Parse(externalAPIResponce.Responce), RequestReciveTime = DateTime.Now };
                    _context.AsyncUserTransactionsDBS.Add(asyncUserTransactions);
                    _context.SaveChanges();

                    break;
                case "GetNumberOfDaysOutsideCoun":
                    
                    AsyncDaysOutsideCoun asyncDaysOutsideCoun = new AsyncDaysOutsideCoun() { RequestStatuesID = requestStatues, DaysOutsideCoun = Int32.Parse(externalAPIResponce.Responce), RequestReciveTime = DateTime.Now };
                    _context.AsyncDaysOutsideCounDBS.Add(asyncDaysOutsideCoun);
                    _context.SaveChanges();

                    break;
                case "GetAge":

                    AsyncAge asyncAge = new AsyncAge() { RequestStatuesID = requestStatues, Age = Int32.Parse(externalAPIResponce.Responce), RequestReciveTime = DateTime.Now };
                    _context.AsyncAgeDBS.Add(asyncAge);
                    _context.SaveChanges();

                    break;
            }
            checkAlldataReady();
        }

        private void checkAlldataReady()
        {
            throw new NotImplementedException();
        }
    }
}