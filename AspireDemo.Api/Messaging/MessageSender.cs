using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;

namespace AspireDemo.Api.Messaging;

public interface IMessageSender<T>
{
    Task SendMessage(T message);
}

public class MessageSender<T>(
    string queueName,
    IConnection connection,
    ILogger<MessageSender<T>> logger) : IMessageSender<T>
{
    private static readonly ActivitySource ActivitySource = new(nameof(MessageSender<T>));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public Task SendMessage(T message)
    {
        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        var activityName = $"{queueName} send";

        using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Producer);
        using var channel = connection.CreateModel();
        var props = channel.CreateBasicProperties();

        ActivityContext contextToInject = default;
        if (activity != null)
        {
            contextToInject = activity.Context;
        }
        else if (Activity.Current != null)
        {
            contextToInject = Activity.Current.Context;
        }

        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), props, InjectTraceContextIntoBasicProperties);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.destination", "");
        activity?.SetTag("messaging.rabbitmq.routing_key", queueName);

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: props, body: body);

        return Task.CompletedTask;
    }

    private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        try
        {
            if (props.Headers == null)
            {
                props.Headers = new Dictionary<string, object>();
            }

            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to inject properties.");
        }
    }
}
