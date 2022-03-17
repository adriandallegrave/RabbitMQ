using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var chatRooms = new List<string>
{
    "Sports",
    "Politics",
    "General"
};

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var message = $"SUCCESS: exchange created";
var messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);

foreach (var room in chatRooms)
{
    channel.ExchangeDeclare("EXCHANGE.CHAT_APP", ExchangeType.Direct, true);
    channel.QueueDeclare($"QUEUE.ADMIN.CHAT_APP.{room}", false, false, true);
    channel.QueueBind($"QUEUE.ADMIN.CHAT_APP.{room}", "EXCHANGE.CHAT_APP", $"CHANNEL.{room}");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, eventArgs) =>
    {
        var body = eventArgs.Body.ToArray();
        var msg = System.Text.Encoding.UTF8.GetString(body);
        Console.WriteLine(msg);
    };

    channel.BasicConsume($"QUEUE.ADMIN.CHAT_APP.{room}", true, consumer);

    channel.BasicPublish($"EXCHANGE.CHAT_APP", $"CHANNEL.{room}", null, messageInBytes);
}

Console.ReadLine();

channel.Close();
connection.Close();
