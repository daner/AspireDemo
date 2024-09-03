using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AspireDemo.EmailWorker;

public abstract class AbstractRabbitWorker<T> : BackgroundService
{
    public const string RetryPolicy = "worker-retry-policy";
    public const string ActivitySourceName = nameof(AbstractRabbitWorker<T>);

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly ResiliencePipeline _resiliencePipeline;
    private readonly IModel _channel;

    protected readonly ILogger<AbstractRabbitWorker<T>> _logger;

    public AbstractRabbitWorker(string queueName, IConnection connection, ResiliencePipeline resiliencePipeline, ILogger<AbstractRabbitWorker<T>> logger)
    {
        _queueName = queueName;
        _connection = connection;
        _resiliencePipeline = resiliencePipeline;
        _logger = logger;
        _channel = connection.CreateModel();
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
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.destination", "");
            activity?.SetTag("messaging.rabbitmq.routing_key", _queueName);

            try
            {
                var body = ea.Body.ToArray();
                var content = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message {@message}", content);

                activity?.SetTag("message", content);

                var message = JsonSerializer.Deserialize<T>(content);

                if (message != null)
                {
                    await _resiliencePipeline.ExecuteAsync(async token =>
                    {
                        await HandleMessage(message, stoppingToken);
                    });
                }
                else
                {
                    _logger.LogError("Could not deserialize message {@message} to type {type}", content, typeof(T).Name);
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message processing failed.");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume(_queueName, false, consumer);

        return Task.CompletedTask;
    }

    protected abstract Task HandleMessage(T content, CancellationToken stoppingToken);
    protected virtual Task OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    protected virtual Task OnConsumerUnregistered(object? sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    protected virtual Task OnConsumerRegistered(object? sender, ConsumerEventArgs e) { return Task.CompletedTask; }
    protected virtual Task OnConsumerShutdown(object? sender, ShutdownEventArgs e) { return Task.CompletedTask; }
    protected virtual void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e) { }

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
