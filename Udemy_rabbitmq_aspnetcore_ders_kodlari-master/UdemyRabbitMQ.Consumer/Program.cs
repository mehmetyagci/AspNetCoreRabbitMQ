using Newtonsoft.Json;
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

    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://mgujdhwy:XPjqFTiaVobxvbhF6q3AIK8H4gK251Ke@spider.rmq.cloudamqp.com/mgujdhwy");

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
                             string message = Encoding.UTF8.GetString(ea.Body);

                             User u = JsonConvert.DeserializeObject<User>(message);

                             Console.WriteLine($"gelen mesaj:{u.Id.ToString()}-{u.Name}-{u.Email}-{u.Password}");

                             channel.BasicAck(ea.DeliveryTag, multiple: false);
                         };
                    Console.WriteLine("Çıkış yapmak tıklayınız..");
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