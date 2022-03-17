using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Creating connection

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

Console.WriteLine("Choose a username: ");
string? USERNAME = Console.ReadLine();

// Creating a queue and consuming

channel.QueueDeclare($"QUEUE.{USERNAME}.CHAT_APP", false, false, true);
channel.QueueBind($"QUEUE.{USERNAME}.CHAT_APP", "EXCHANGE.CHAT_APP", "");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var msg = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine(msg);
};

channel.BasicConsume($"QUEUE.{USERNAME}.CHAT_APP", true, consumer);


// Finishing it up

bool keepGoing = true;
string message = "n";
var messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);

while (keepGoing)
{
    message = Console.ReadLine()!;
    if (message == "quit")
    {
        keepGoing = false;
    }
    message = $"{USERNAME}: {message}";
    messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("EXCHANGE.CHAT_APP", "", null, messageInBytes);
}

channel.Close();
connection.Close();








/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
*/








/*
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


*/