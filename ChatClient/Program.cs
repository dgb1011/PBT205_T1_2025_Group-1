using System;
using System.Text;
using RabbitMQ.Client;


var factory = new ConnectionFactory{ HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare
    (Queue: "Queue1",
    durable: false, 
    autoDelete: false,
    arguments: null);

Console.WriteLine("Enter messages to send. Type 'exit' to quit.");

while (true)
{
    Console.Write("You: ");
    string message = Console.ReadLine();

    if (message.ToLower() == "exit")
        break; 

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "", routingKey: "Queue1", basicProperties: null, body: body);

    Console.WriteLine($"Sent message: {message}");
}