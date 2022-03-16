using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Text;

namespace ExploreCalifornia.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        [HttpPost]
        [Route("Book")]
        public IActionResult Book()
        {
            var tourname = Request.Form["tourname"];
            var name = Request.Form["name"];
            var email = Request.Form["email"];
            var needsTransport = Request.Form["transport"] == "on";

            var message = $"{tourname};{name};{email}";
            SendMessage("tour.booked", message);

            return Redirect($"/BookingConfirmed?tourname={tourname}&name={name}&email={email}");
        }

        [HttpPost]
        [Route("Cancel")]
        public IActionResult Cancel()
        {
            var tourname = Request.Form["tourname"];
            var name = Request.Form["name"];
            var email = Request.Form["email"];
            var cancelReason = Request.Form["reason"];

            var message = $"{tourname};{name};{email};{cancelReason}";
            SendMessage("tour.canceled", message);

            return Redirect($"/BookingCanceled?tourname={tourname}&name={name}");
        }

        private void SendMessage(string routingKey, string message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // This change alone is useless
            // It's needed to access RabbitMQ and delete the current exchange first
            // Because it's already registered as a fanout type

            //channel.ExchangeDeclare("webappExchange", ExchangeType.Direct, true);

            var bytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("webappExchange", routingKey, null, bytes);

            channel.Close();
            connection.Close();
        }
    }
}