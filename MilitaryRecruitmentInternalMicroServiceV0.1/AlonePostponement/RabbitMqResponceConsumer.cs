using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
namespace AlonePostponement
{
    public class RabbitMqConsumer
    {
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public RabbitMqConsumer(String Hostname)
        {
            this.factory = new ConnectionFactory() { HostName = Hostname};
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserRequestExch", ExchangeType.Direct);

            var queName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queName, exchange: "UserRequestExch", routingKey: "AlonePostponement");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var mess = Encoding.UTF8.GetString(body);
                int ID = Int32.Parse(mess);
                h(ID);
            };
            channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);
        }
        public void Register()
        {
            
        }

        public static void h(int ID)
        {

            Console.WriteLine("hi" + ID);

        }
    }
}


