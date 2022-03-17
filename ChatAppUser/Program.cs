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

Console.WriteLine("Choose a username: ");
string username = Console.ReadLine()!;
Console.Clear();

Console.WriteLine("Available chat rooms: Sports, Politics, General");
Console.WriteLine("Choose a chatroom: ");
string chosenRoom = Console.ReadLine()!;
if (!chatRooms.Contains(chosenRoom)) chosenRoom = "General";
Console.Clear();

channel.QueueDeclare($"QUEUE.{username}.CHAT_APP.{chosenRoom}", false, false, true);
channel.QueueBind($"QUEUE.{username}.CHAT_APP.{chosenRoom}", $"EXCHANGE.CHAT_APP", $"CHANNEL.{chosenRoom}");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var msg = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine(msg);
};

channel.BasicConsume($"QUEUE.{username}.CHAT_APP.{chosenRoom}", true, consumer);


string message = $"{username} joined the chat";
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
        message = $"{username} left the room";
    }
    message = keepGoing ? $"{username}: {message}" : $"{message}";
    messageInBytes = System.Text.Encoding.UTF8.GetBytes(message);
    channel.BasicPublish($"EXCHANGE.CHAT_APP", $"CHANNEL.{chosenRoom}", null, messageInBytes);
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
