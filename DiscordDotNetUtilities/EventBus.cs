using DiscordDotNetUtilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordDotNetUtilities;

public class EventBus(IServiceProvider serviceProvider, ILogger<EventBus> logger) : IEventBus
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        var handlers = serviceProvider.GetServices<IEventHandler<T>>();

        var tasks = handlers.Select(handler =>
            Task.Run(async () =>
            {
                try
                {
                    await handler.HandleAsync(@event, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception in {HandlerType} handling {EventType}: {ExceptionMessage}",
                        handler.GetType().Name, typeof(T).Name, ex.Message);
                }
            }, cancellationToken));

        await Task.WhenAll(tasks);
    }
}