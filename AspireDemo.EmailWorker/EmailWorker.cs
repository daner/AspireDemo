using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace AspireDemo.EmailWorker;

public record EmailMessage(string From, string To, string Subject, string Body);

//TODO: Factor out common stuff to an AbstractRabbitWorker
public class EmailWorker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(nameof(EmailWorker));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private readonly ILogger<EmailWorker> _logger;
    private readonly IConnection _connection;
    private readonly ISmtpClient smtpClient;
    private readonly IOptions<EmailOptions> emailOptions;
    private readonly IModel _channel;

    public EmailWorker(ILogger<EmailWorker> logger, IConnection connection, ISmtpClient smtpClient, IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _connection = connection;
        this.smtpClient = smtpClient;
        this.emailOptions = emailOptions;
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
            var parentContext = Propagator.Extract(default, ea.BasicProperties, ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;
            var activityName = $"{ea.RoutingKey} receive";

            using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);

            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                activity?.SetTag("message", message);
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination_kind", "queue");
                activity?.SetTag("messaging.destination", "");
                activity?.SetTag("messaging.rabbitmq.routing_key", "email");

                await HandleMessage(message);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Message processing failed.");
            }
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume("email", false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessage(string content)
    {
        _logger.LogInformation("Received message {@message}", content);

        var message = JsonSerializer.Deserialize<EmailMessage>(content);

        if (message == null)
        {
            return;
        }

        var mailMessage = new MailMessage(emailOptions.Value.From, message.To, message.Subject, message.Body)
        {
            IsBodyHtml = true
        };

        await smtpClient.SendMailAsync(mailMessage);
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


    private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
    {
        try
        {
            if (props.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes!)];
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract trace context.");
        }

        return Enumerable.Empty<string>();
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
