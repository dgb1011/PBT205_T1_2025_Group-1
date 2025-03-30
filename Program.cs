using RabbitMQ.Client;
using System;
using System.Text;

class SendOrder
{
    static void Main(string[] args)
    {
        if (args.Length != 5)
        {
            Console.WriteLine("Usage: sendOrder <username> <middleware_endpoint> <side> <quantity> <price>");
            return;
        }

        string username = args[0];
        string middlewareEndpoint = args[1];
        string side = args[2].ToUpper();
        int quantity = int.Parse(args[3]);
        double price = double.Parse(args[4]);

        if (side != "BUY" && side != "SELL")
        {
            Console.WriteLine("Side must be BUY or SELL");
            return;
        }

        var factory = new ConnectionFactory() { HostName = middlewareEndpoint };
        try
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "orders", type: "topic", durable: true, autoDelete: false);

                    string message = $"{username},{side},{quantity},{price}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "orders",
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine($"Sent order: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending order: {ex.Message}");
        }
    }
}
