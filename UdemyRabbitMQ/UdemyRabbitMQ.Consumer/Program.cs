using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.Consumer
{
    public enum LogNames
    {
        Critical,
        Error,
        Warning
    }

    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://qwrsqoyx:zgzgFvdx_vVf0cK_HS0blV8s9Ir9bCGD@chinook.rmq.cloudamqp.com/qwrsqoyx");

            //factory.HostName = "localhost";  // Local RabbitMQ servisine bağlanma

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

                    channel.QueueDeclare("kuyruk1", false, false, false, null);

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    headers.Add("x-match", "any");

                    channel.QueueBind("kuyruk1", "header-exchange", string.Empty, headers);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("kuyruk1", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                        Console.WriteLine($"gelen mesaj:{message}");

                        channel.BasicAck(ea.DeliveryTag, multiple: false); // Mesajı kuyruktan silebilirsin.
                    };
                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
