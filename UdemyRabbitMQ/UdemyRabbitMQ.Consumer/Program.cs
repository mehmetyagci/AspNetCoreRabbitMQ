using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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
                    //channel.QueueDeclare("task_queue", durable: true, exclusive: false, autoDelete: false, null);

                    channel.ExchangeDeclare("topic-exchange", durable: true, type: ExchangeType.Topic);

                    var queueName = channel.QueueDeclare().QueueName;

                    string routingKey = "#.Warning";

                    channel.QueueBind(queue: queueName, exchange: "topic-exchange", routingKey: routingKey);

                    //foreach (var item in Enum.GetNames(typeof(LogNames)))
                    //{
                    //    channel.QueueBind(queue: queueName, exchange: "topic-exchange", routingKey: item);
                    //}

                    // Bana bir tane mesaj gelsin ve bu mesajı hallettikten sonra bir sonraki gelsin.
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);

                    Console.WriteLine("Custom log bekliyorum...");

                    var consumer = new EventingBasicConsumer(channel);

                    /// autoAck: false kuyruğu oto. silme, ben silicem.
                    //channel.BasicConsume("task_queue", autoAck: false, consumer);
                    channel.BasicConsume(queueName, false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var log = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine("Log alındı:" + log);

                        int time = int.Parse(GetMessage(args));
                        Thread.Sleep(time);

                        File.AppendAllText("logs_critical_error.txt", log + "\n");

                        Console.WriteLine("loglama bitti:" + log);

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
