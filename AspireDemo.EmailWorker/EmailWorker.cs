using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AspireDemo.EmailWorker;

//TODO: Factor out common stuff to an AbstractRabbitWorker
public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public EmailWorker(ILogger<EmailWorker> logger, IConnection connection)
    {
        _logger = logger;
        _connection = connection;
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "email", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await HandleMessage(message);
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume("email", false, consumer);

        return Task.CompletedTask;
    }

    private Task HandleMessage(string content)
    {
        _logger.LogInformation("Received message {@message}", content);
        return Task.CompletedTask;
    }

    private Task OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerConsumerCancelled));
        return Task.CompletedTask;
    }
    private Task OnConsumerUnregistered(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerUnregistered));
        return Task.CompletedTask;
    }
    private Task OnConsumerRegistered(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerRegistered));
        return Task.CompletedTask;
    }
    private Task OnConsumerShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerShutdown));
        return Task.CompletedTask;
    }
    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(RabbitMQ_ConnectionShutdown));
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
