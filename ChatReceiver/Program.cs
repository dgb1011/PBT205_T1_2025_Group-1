using System;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
using System.Text;

/*class ChatReceiver
{
   private const string QUEUE_NAME = "chat_room";
   private const string RABBITMQ_HOST = "localhost";

  static void Main()
   {
       var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
       using var connection = factory.CreateConnection();
       using var channel = connection.CreateModel();

       channel.QueueDeclare(queue: QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);

       var consumer = new EventingBasicConsumer(channel);
       consumer.Received += (model, ea) =>
       {
           var body = ea.Body.ToArray();
           var message = Encoding.UTF8.GetString(body);
           Console.WriteLine($"\n{message}");
       };

       channel.BasicConsume(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

       Console.WriteLine("Listening for messages... Press [Enter] to exit.");
       Console.ReadLine();
   }
}
*/