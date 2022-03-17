using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Creating exchange

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.ExchangeDeclare("EXCHANGE.CHAT_APP", ExchangeType.Fanout, true);


// Creating a queue and consuming

channel.QueueDeclare("QUEUE.ADMIN.CHAT_APP", false, false, true);
channel.QueueBind("QUEUE.ADMIN.CHAT_APP", "EXCHANGE.CHAT_APP", "");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var msg = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine(msg);
};

channel.BasicConsume("QUEUE.ADMIN.CHAT_APP", true, consumer);


// Sending confirmation message

var message = $"SUCCESS: exchange created";
var messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);
channel.BasicPublish("EXCHANGE.CHAT_APP", "", null, messageInBytes);

Console.ReadLine();

channel.Close();
connection.Close();
