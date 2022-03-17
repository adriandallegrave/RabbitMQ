using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Creating connection

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

Console.WriteLine("Choose a username: ");
string USERNAME = Console.ReadLine()!;
Console.Clear();

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

string message = $"{USERNAME} joined the chat";
var messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);
channel.BasicPublish("EXCHANGE.CHAT_APP", "", null, messageInBytes);

bool keepGoing = true;

while (keepGoing)
{
    message = Console.ReadLine()!;
    
    ClearCurrentConsoleLine();

    if (message == "quit")
    {
        keepGoing = false;
        message = $"{USERNAME} left the room";
    }
    message = keepGoing ? $"{USERNAME}: {message}" : $"{message}";
    messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("EXCHANGE.CHAT_APP", "", null, messageInBytes);
}

channel.Close();
connection.Close();

static void ClearCurrentConsoleLine()
{
    Console.SetCursorPosition(0, Console.CursorTop - 1);
    int currentCursorLine = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentCursorLine);
}
