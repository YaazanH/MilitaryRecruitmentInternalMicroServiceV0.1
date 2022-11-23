using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using ExternalAPICaller.Models;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ExternalAPICaller
{

    public class RabbitMqAPIRequestConsumer
    {

        private async Task<string> APICall(string GURI,string Token)
        {


            Uri uri = new Uri(GURI);

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var Client = new HttpClient(clientHandler))
            {
                Client.BaseAddress = uri;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                using (HttpResponseMessage response = await Client.GetAsync(Client.BaseAddress))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMqAPIRequestConsumer(String Hostname)
        {
            this.factory = new ConnectionFactory() { HostName = Hostname };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();


            channel.QueueDeclare(queue: "requestQueue",exclusive:false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var properties = channel.CreateBasicProperties();

                properties.CorrelationId=ea.BasicProperties.CorrelationId;

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);
            
                RabbitMQobj rabbitMQobj = JsonSerializer.Deserialize<RabbitMQobj>(recmess);

                var resp=APICall(rabbitMQobj.URL, rabbitMQobj.JWT);

                RabbitMQResponce rabbitMQResponce = new RabbitMQResponce();

                rabbitMQResponce.ProcID = rabbitMQobj.ProcID;
                rabbitMQResponce.Responce = resp.ToString();

                var mess = JsonSerializer.Serialize(rabbitMQResponce);
                var body = Encoding.UTF8.GetBytes(mess);
                
                channel.BasicPublish("",ea.BasicProperties.ReplyTo,properties,body);

            };
            channel.BasicConsume(queue: "requestQueue", autoAck: true, consumer: consumer);

            Console.ReadKey();

        }
    }
}


