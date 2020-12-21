using RabbitMQ.Client;
using System;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{

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

                    channel.ExchangeDeclare("direct-exchange",durable:true, type: ExchangeType.Direct);

                    Array log_name_array = Enum.GetValues(typeof(LogNames));

                    for (int i = 1; i < 11; i++)
                    {
                        Random rnd = new Random();

                        LogNames log = (LogNames)log_name_array.GetValue(rnd.Next(log_name_array.Length));

                        var bodyByte = Encoding.UTF8.GetBytes($"log={log.ToString()}-{i}");

                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true; // Mesajınıda sağlama aldık.

                        //channel.BasicPublish("", routingKey: "task_queue", properties, body: bodyByte);

                        channel.BasicPublish("direct-exchange", routingKey: log.ToString(), properties, body: bodyByte);

                        Console.WriteLine($"log mesajı gönderilmiştir.{log.ToString()}-{i}");
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
