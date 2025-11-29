using chackgpt.Web.Hubs;
using chackgpt.Web.Models;
using System.Text.Json;

namespace chackgpt.Web.Services;

/// <summary>
/// Service that listens to DisplayVideoService events and broadcasts them via SignalR to all connected clients.
/// Uses generic BroadcastHostedService pattern to eliminate code duplication.
/// </summary>
/// <remarks>
/// C# 14 Primary Constructor - passes dependencies to base class
/// </remarks>
public class VideoBroadcastService(
    Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext,
    IVideoService displayVideoService,
    ILogger<VideoBroadcastService> logger)
    : BroadcastHostedService<IVideoService, ChatHub>(hubContext, displayVideoService, logger)
{
    protected override Task SubscribeToEventsAsync(CancellationToken cancellationToken)
    {
        EventSource.VideoDisplayRequested += OnVideoDisplayRequested;
        EventSource.VideoCloseRequested += OnVideoCloseRequested;
        Logger.LogInformation("📢 VideoBroadcastService initialized and listening for video display events");
        return Task.CompletedTask;
    }

    protected override Task UnsubscribeFromEventsAsync()
    {
        EventSource.VideoDisplayRequested -= OnVideoDisplayRequested;
        EventSource.VideoCloseRequested -= OnVideoCloseRequested;
        return Task.CompletedTask;
    }

    private async void OnVideoDisplayRequested(object? sender, VideoDisplayInfo videoInfo)
    {
        string videoJson = JsonSerializer.Serialize(videoInfo);
        Logger.LogInformation("📡 Broadcasting VideoDisplayRequested to all SignalR clients: {Title}", videoInfo.Title);
        await BroadcastAsync("VideoDisplayRequested", videoJson);
    }

    private async void OnVideoCloseRequested(object? sender, EventArgs e)
    {
        Logger.LogInformation("📡 Broadcasting VideoCloseRequested to all SignalR clients");
        await BroadcastAsync("VideoCloseRequested");
    }
}
