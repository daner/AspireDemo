using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;

namespace AspireDemo.EmailWorker;

public record EmailMessage(string From, string To, string Subject, string Body);

public class EmailWorker(
    ILogger<EmailWorker> logger,
    IConnection connection,
    ISmtpClient smtpClient,
    IOptions<EmailOptions> emailOptions,
    [FromKeyedServices(AbstractRabbitWorker<EmailMessage>.RetryPolicy)] ResiliencePipeline resiliencePipeline) 
    : AbstractRabbitWorker<EmailMessage>(QueueName, connection, resiliencePipeline, logger)
{
    private const string QueueName = "email";

    protected override async Task HandleMessage(EmailMessage message, CancellationToken stoppingToken)
    {
        var mailMessage = new MailMessage(emailOptions.Value.From, message.To, message.Subject, message.Body)
        {
            IsBodyHtml = true
        };

        await smtpClient.SendMailAsync(mailMessage, stoppingToken);
    }

    protected override Task OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerConsumerCancelled));
        return Task.CompletedTask;
    }
    protected override Task OnConsumerUnregistered(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerUnregistered));
        return Task.CompletedTask;
    }
    protected override Task OnConsumerRegistered(object? sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerRegistered));
        return Task.CompletedTask;
    }
    protected override Task OnConsumerShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(OnConsumerShutdown));
        return Task.CompletedTask;
    }
    protected override void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("{Method}", nameof(RabbitMQ_ConnectionShutdown));
    }
}
