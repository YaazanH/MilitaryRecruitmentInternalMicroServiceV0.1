using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ExternalAPICaller.Models;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;


namespace ExternalAPICaller.BackgroundServices
{
    public class RabbitMQserv : BackgroundService
    {
        //private readonly AlonePostponementContext _context;
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMQserv(IServiceScopeFactory factory)
        {
           // _context = factory.CreateScope().ServiceProvider.GetRequiredService<AlonePostponementContext>();
        }
        private async Task<string> APICall(string GURI, string Token)
        {

            try
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
                            return System.Text.Json.JsonSerializer.Serialize("not avalible");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
                return System.Text.Json.JsonSerializer.Serialize("not avalible") ;
            }
            
            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            this.factory = new ConnectionFactory() { HostName = "host.docker.internal" };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();


            channel.QueueDeclare(queue: "requestQueue", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var properties = channel.CreateBasicProperties();

                properties.CorrelationId = ea.BasicProperties.CorrelationId;

                var recbody = ea.Body.ToArray();

                var recmess = Encoding.UTF8.GetString(recbody);

                RabbitMQobj rabbitMQobj = System.Text.Json.JsonSerializer.Deserialize<RabbitMQobj>(recmess);
                                
                var resp = JsonConvert.DeserializeObject<string>(await APICall(rabbitMQobj.URL, rabbitMQobj.JWT));
                RabbitMQResponce rabbitMQResponce = new RabbitMQResponce();

                rabbitMQResponce.ProcID = rabbitMQobj.ProcID;
                rabbitMQResponce.Responce = resp;

                var mess = System.Text.Json.JsonSerializer.Serialize(rabbitMQResponce);
                var body = Encoding.UTF8.GetBytes(mess);

                channel.BasicPublish("", ea.BasicProperties.ReplyTo, properties, body);

            };
            channel.BasicConsume(queue: "requestQueue", autoAck: true, consumer: consumer);


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
