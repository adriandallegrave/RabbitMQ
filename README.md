# RabbitMQ

WebApp:       program.cs      → Creates an Exchange
              controllers.cs  → Publish messages
emailService: program.cs      → Creates queue, binds it and consume only booking messages
backOffice:   program.cs      → Creates queue, binds it and consume all messages

