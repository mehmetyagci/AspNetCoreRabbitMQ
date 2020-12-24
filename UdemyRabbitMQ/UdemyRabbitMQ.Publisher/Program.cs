using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    // Critical.Error.Info  Info.Warning.Critical
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
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
                    // durable:true kuyruğu sağlama alıyor. PC restart bile olsa kaybolmuyor.
                    // channel.QueueDeclare("task_queue", durable: true, false, false, null);
                    channel.ExchangeDeclare("header-exchange",durable:true, type: ExchangeType.Headers);

                    var properties = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;
                    Console.WriteLine("Mesaj gönderildi.");

                    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
                }
            }

            Console.WriteLine("Çıkış yapmak için tıklayınız.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
