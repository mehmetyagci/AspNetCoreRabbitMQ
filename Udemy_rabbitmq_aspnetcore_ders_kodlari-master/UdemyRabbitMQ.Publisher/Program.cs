using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    //Critical.Error.Info Info.Warning.Critical
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
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

                    var properties = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format5", "pdf");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;

                    User u = new User() { Id = 1, Name = "Fatih", Email = "f-cakiroglu@outlook.com", Password = "1234" };

                    String userSerialize = JsonConvert.SerializeObject(u);

                    Console.WriteLine("mesaj gönderildi");
                    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(userSerialize));
                }

                Console.WriteLine("Çıkış yapmak tıklayınız..");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}