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

            //factory.Uri = new Uri("amqps://qwrsqoyx:zgzgFvdx_vVf0cK_HS0blV8s9Ir9bCGD@chinook.rmq.cloudamqp.com/qwrsqoyx");

             factory.HostName = "localhost";  // Local RabbitMQ servisine bağlanma

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    string message = "Hello World";

                    var bodyByte = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", routingKey: "hello", null, body: bodyByte);

                    Console.WriteLine("Mesajınız gönderilmiştir.");
                }
            }

            Console.WriteLine("Çıkış yapmak için tıklayınız.");
            Console.ReadLine();
        }
    }
}
