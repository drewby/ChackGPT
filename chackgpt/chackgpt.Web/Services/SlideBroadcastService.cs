using chackgpt.Web.Hubs;
using chackgpt.Web.Models;
using System.Text.Json;

namespace chackgpt.Web.Services;

/// <summary>
/// Service that broadcasts slide display events to all connected SignalR clients.
/// Uses generic BroadcastHostedService pattern to eliminate code duplication.
/// </summary>
/// <remarks>
/// C# 14 Primary Constructor - passes dependencies to base class
/// </remarks>
public class SlideBroadcastService(
    Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext,
    ISlideService displaySlideService,
    ILogger<SlideBroadcastService> logger)
    : BroadcastHostedService<ISlideService, ChatHub>(hubContext, displaySlideService, logger)
{
    protected override Task SubscribeToEventsAsync(CancellationToken cancellationToken)
    {
        EventSource.SlideDisplayRequested += OnSlideDisplayRequested;
        EventSource.SlideCloseRequested += OnSlideCloseRequested;
        Logger.LogInformation("📡 SlideBroadcastService initialized and subscribed to slide display events");
        return Task.CompletedTask;
    }

    protected override Task UnsubscribeFromEventsAsync()
    {
        EventSource.SlideDisplayRequested -= OnSlideDisplayRequested;
        EventSource.SlideCloseRequested -= OnSlideCloseRequested;
        return Task.CompletedTask;
    }

    private async void OnSlideDisplayRequested(object? sender, SlideDisplayInfo slideInfo)
    {
        Logger.LogInformation("🔊 Broadcasting slide display to all clients: {Topic} - Slide {SlideNumber}", 
            slideInfo.Topic, slideInfo.SlideNumber);
        
        // Serialize slide info to JSON for transmission
        string slideJson = JsonSerializer.Serialize(slideInfo);
        await BroadcastAsync("SlideDisplayRequested", slideJson);
    }

    private async void OnSlideCloseRequested(object? sender, EventArgs e)
    {
        Logger.LogInformation("🔊 Broadcasting slide close to all clients");
        await BroadcastAsync("SlideCloseRequested");
    }
}
