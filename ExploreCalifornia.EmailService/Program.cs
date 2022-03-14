using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Run this console application and make a booking again
// The message will appear in the console
// If this console app is closed, the message will appear when it opens again

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare("emailServiceQueue", true, false, false);
channel.QueueBind("emailServiceQueue", "webappExchange", "");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var msg = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine(msg);
};

channel.BasicConsume("emailServiceQueue", true, consumer);

Console.ReadLine();

channel.Close();
connection.Close();