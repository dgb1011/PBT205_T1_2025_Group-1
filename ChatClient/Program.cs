using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


class Program
{
    static async Task Main()
    {
        await SendMessageAsync();
        await ReceiveMessagesAsync();
    }

    static async Task SendMessageAsync() // add custom message
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync
             (queue: "Chat Room",
             durable: false,
             exclusive: false,
             autoDelete: false,
             arguments: null);

        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "Chat Room", body: body);
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
    static async Task ReceiveMessagesAsync() 
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync
            (queue: "Chat Room", 
            durable: false, 
            exclusive: false, 
            autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync("Chat Room", autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
    //add login
}