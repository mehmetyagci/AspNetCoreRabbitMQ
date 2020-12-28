using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spire.Doc;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Udemy_Word_To_Pdf.Consumer
{
    class Program
    {

        public static bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                memoryStream.Position = 0;

                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

                Attachment attach = new Attachment(memoryStream, ct);
                attach.ContentDisposition.FileName = $"{fileName}.pdf";

                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                mailMessage.From = new MailAddress("mehmetyagci53@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Pdf Dosyası oluşturuldu.";
                mailMessage.Body = "pdf dosyanız ektedir.";

                mailMessage.IsBodyHtml = true;

                mailMessage.Attachments.Add(attach);


                //smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("mehmetyagci53@gmail.com", "Mhm.571622");
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = NetworkCred;
                smtpClient.Port = 587;
                smtpClient.Send(mailMessage);

                Console.WriteLine($"Sonuç: {email} adresine gönderilmiştir.");

                memoryStream.Close();
                memoryStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mail gönderim sırasında bir hata meydana geldi! {ex.ToString()}");
                return false;
            }
        }

        static void Main(string[] args)
        {
            bool result = false;
            var factory = new ConnectionFactory();

            factory.Uri = new Uri("amqps://qwrsqoyx:zgzgFvdx_vVf0cK_HS0blV8s9Ir9bCGD@chinook.rmq.cloudamqp.com/qwrsqoyx");

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert-exchange", ExchangeType.Direct, true, false, null);

                    channel.QueueBind(queue: "File", exchange: "convert-exchange", routingKey: "WordToPdf");

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("File", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Console.WriteLine("Kuyruktan bir mesaj alındı");

                            Document document = new Document();

                            string deserializeString = Encoding.UTF8.GetString(ea.Body.ToArray());

                            MessageWordToPdf messageWordToPdf = JsonConvert.DeserializeObject<MessageWordToPdf>(deserializeString);

                            document.LoadFromStream(new MemoryStream(messageWordToPdf.WordByte), FileFormat.Docx2013);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                document.SaveToStream(ms, FileFormat.PDF);

                                result = EmailSend(messageWordToPdf.Email, ms, messageWordToPdf.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Hata meydana geldi:" + ex.ToString());
                            throw;
                        }

                        if (result)
                        {
                            Console.WriteLine("Kuyruktan Mesaj başarılı bir şekilde işlendi...");
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    };
                }
            }

            Console.WriteLine("Çıkmak için tıklayınız!!!");
            Console.ReadLine();
        }
    }
}
