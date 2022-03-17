using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare("backOfficeQueue", true, false, false);
var headers = new Dictionary<string, object>
{
    { "subject", "tour" },
    { "action", "booked" },
    { "x-match", "any" }
};
channel.QueueBind("backOfficeQueue", "webappExchange", "", headers);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, eventArgs) =>
{
    var msg = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
#pragma warning disable CS8604 // Possible null reference argument.
    var subject = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["subject"] as byte[]);
    var action = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["action"] as byte[]);
#pragma warning restore CS8604 // Possible null reference argument.
    Console.WriteLine($"{subject}.{action}: {msg}");
};

channel.BasicConsume("backOfficeQueue", true, consumer);

Console.ReadLine();

channel.Close();
connection.Close();
