using RabbitMQ.Client;
using System;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
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

                    channel.ExchangeDeclare("logs",durable:true, type: ExchangeType.Fanout);

                    string message = GetMessage(args);  //"Hello World";

                    for (int i = 0; i < 10; i++)
                    {
                        var bodyByte = Encoding.UTF8.GetBytes($"{message}-{i}");

                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true; // Mesajınıda sağlama aldık.

                        //channel.BasicPublish("", routingKey: "task_queue", properties, body: bodyByte);

                        channel.BasicPublish("logs", routingKey: "", properties, body: bodyByte);

                        Console.WriteLine($"Mesajınız gönderilmiştir.{message}-{i}");
                    }
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
