using chackgpt.Web.Hubs;

namespace chackgpt.Web.Services;

/// <summary>
/// Service that broadcasts ChackGPT emotion changes to all connected SignalR clients.
/// Uses generic BroadcastHostedService pattern to eliminate code duplication.
/// </summary>
/// <remarks>
/// C# 14 Primary Constructor - passes dependencies to base class
/// </remarks>
public class EmotionBroadcastService(
    Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext,
    IEmotionService<Models.ChackEmotion> emotionService,
    ILogger<EmotionBroadcastService> logger)
    : BroadcastHostedService<IEmotionService<Models.ChackEmotion>, ChatHub>(hubContext, emotionService, logger)
{
    protected override Task SubscribeToEventsAsync(CancellationToken cancellationToken)
    {
        EventSource.EmotionChanged += OnEmotionChanged;
        Logger.LogInformation("📡 EmotionBroadcastService initialized and subscribed to emotion changes");
        return Task.CompletedTask;
    }

    protected override Task UnsubscribeFromEventsAsync()
    {
        EventSource.EmotionChanged -= OnEmotionChanged;
        return Task.CompletedTask;
    }

    private async void OnEmotionChanged(object? sender, Models.ChackEmotion emotion)
    {
        var emotionString = emotion.ToString().ToLowerInvariant();
        Logger.LogInformation("🔊 Broadcasting emotion change to all clients: {Emotion}", emotionString);
        await BroadcastAsync("EmotionChanged", emotionString);
    }
}
