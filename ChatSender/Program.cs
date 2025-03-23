//using RabbitMQ.Client;
using System.Text;
/*
class ChatSender
{
    private const string QUEUE_NAME = "chat_room";
    private const string RABBITMQ_HOST = "localhost";

    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = RABBITMQ_HOST };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);

        Console.Write("Enter your username: ");
        string username = Console.ReadLine();

        Console.WriteLine("Type your messages below. Type 'exit' to quit.");

        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit")
                break;

            string fullMessage = $"{username}: {message}";
            var body = Encoding.UTF8.GetBytes(fullMessage);
            //channel.BasicPublish(exchange: "", routingKey: QUEUE_NAME, basicProperties: null, body: body);
        }

        Console.WriteLine("Chat session ended.");
    }
}
   */