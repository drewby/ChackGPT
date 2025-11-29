using Microsoft.AspNetCore.SignalR;

namespace chackgpt.Web.Services;

/// <summary>
/// Generic base class for broadcast services that listen to events and forward them to SignalR clients.
/// This eliminates code duplication across emotion, slide, and video broadcast services.
/// Implements IHostedService for automatic lifecycle management.
/// </summary>
/// <typeparam name="TEventSource">The service that raises events to broadcast</typeparam>
/// <typeparam name="THub">The SignalR hub type for broadcasting</typeparam>
/// <remarks>
/// C# 14 Primary Constructor - eliminates boilerplate field assignments and provides cleaner syntax
/// </remarks>
public abstract class BroadcastHostedService<TEventSource, THub>(
    IHubContext<THub> hubContext,
    TEventSource eventSource,
    ILogger logger) : BackgroundService
    where THub : Hub
{
    /// <summary>
    /// Gets the SignalR hub context for broadcasting messages to clients
    /// </summary>
    protected IHubContext<THub> HubContext { get; } = hubContext;

    /// <summary>
    /// Gets the event source service that this broadcast service listens to
    /// </summary>
    protected TEventSource EventSource { get; } = eventSource;

    /// <summary>
    /// Gets the logger for this broadcast service
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Called when the service is starting. Subscribe to events here.
    /// </summary>
    protected abstract Task SubscribeToEventsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Called when the service is stopping. Unsubscribe from events here.
    /// </summary>
    protected abstract Task UnsubscribeFromEventsAsync();

    /// <summary>
    /// .NET 10 BackgroundService - automatic lifecycle management
    /// Subscribes to events when service starts
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SubscribeToEventsAsync(stoppingToken);
        
        // Keep the service running until cancellation is requested
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// Called when the service is stopping. Ensures clean unsubscription.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await UnsubscribeFromEventsAsync();
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Helper method to broadcast a message to all SignalR clients with error handling
    /// </summary>
    /// <param name="methodName">The SignalR method name to invoke on clients</param>
    /// <param name="arg">The argument to pass to the client method</param>
    protected async Task BroadcastAsync(string methodName, object? arg = null)
    {
        ArgumentNullException.ThrowIfNull(methodName); // C# 11 - concise null argument checks
        
        try
        {
            Logger.LogInformation("📡 Broadcasting {MethodName} to all SignalR clients", methodName);
            if (arg != null)
            {
                await HubContext.Clients.All.SendAsync(methodName, arg, CancellationToken.None);
            }
            else
            {
                await HubContext.Clients.All.SendAsync(methodName, CancellationToken.None);
            }
            Logger.LogInformation("✅ Successfully broadcast {MethodName}", methodName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "❌ Failed to broadcast {MethodName}", methodName);
        }
    }
}
