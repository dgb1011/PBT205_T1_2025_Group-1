using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

class Exchange
{
    private static readonly List<Order> OrderBook = new List<Order>();
    private static readonly object OrderBookLock = new object();

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: exchange <middleware_endpoint>");
            return;
        }

        string middlewareEndpoint = args[0];

        var factory = new ConnectionFactory() { HostName = middlewareEndpoint };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "orders", type: "topic", durable: true, autoDelete: false);
            channel.ExchangeDeclare(exchange: "trades", type: "topic", durable: true, autoDelete: false);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "orders",
                              routingKey: "");

            Console.WriteLine(" [*] Waiting for orders.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received order: {message}");

                string[] parts = message.Split(',');
                if (parts.Length != 4)
                {
                    Console.WriteLine("Invalid order format");
                    return;
                }

                string username = parts[0];
                string side = parts[1].ToUpper();
                int quantity = int.Parse(parts[2]);
                double price = double.Parse(parts[3]);

                Order order = new Order(username, side, quantity, price);

                lock (OrderBookLock)
                {
                    MatchOrder(channel, order);
                }
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    static void MatchOrder(IModel channel, Order order)
    {
        if (order.Side == "BUY")
        {
            // Find matching SELL order
            Order matchingOrder = OrderBook.Find(o => o.Side == "SELL" && o.Price <= order.Price);
            if (matchingOrder != null)
            {
                // Found a match!
                OrderBook.Remove(matchingOrder);
                PublishTrade(channel, order, matchingOrder);
            }
            else
            {
                // No match, add to order book
                OrderBook.Add(order);
                Console.WriteLine($"Added BUY order to book: {order.Username} {order.Side} {order.Quantity} {order.Price}");
            }
        }
        else if (order.Side == "SELL")
        {
            // Find matching BUY order
            Order matchingOrder = OrderBook.Find(o => o.Side == "BUY" && o.Price >= order.Price);
            if (matchingOrder != null)
            {
                // Found a match!
                OrderBook.Remove(matchingOrder);
                PublishTrade(channel, order, matchingOrder);
            }
            else
            {
                // No match, add to order book
                OrderBook.Add(order);
                Console.WriteLine($"Added SELL order to book: {order.Username} {order.Side} {order.Quantity} {order.Price}");
            }
        }
    }

    static void PublishTrade(IModel channel, Order buyOrder, Order sellOrder)
    {
        double tradePrice = sellOrder.Price; // Use the sell order's price for the trade
        string message = $"Trade: {buyOrder.Username} bought from {sellOrder.Username} at {tradePrice}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "trades",
                             routingKey: "",
                             basicProperties: null,
                             body: body);

        Console.WriteLine(message);
    }
}

class Order
{
    public string Username { get; set; }
    public string Side { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    public Order(string username, string side, int quantity, double price)
    {
        Username = username;
        Side = side;
        Quantity = quantity;
        Price = price;
    }
}
