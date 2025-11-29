using Microsoft.Extensions.Logging;

namespace chackgpt.Web.Services;

/// <summary>
/// Service to manage and broadcast ChackGPT's emotional state across the application
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class ChackEmotionService(ILogger<ChackEmotionService> logger) : IEmotionService<Models.ChackEmotion>
{
    private Models.ChackEmotion _currentEmotion = Models.ChackEmotion.Neutral;
    private readonly object _lock = new();
    
    /// <summary>
    /// Event raised when ChackGPT's emotion changes
    /// </summary>
    public event EventHandler<Models.ChackEmotion>? EmotionChanged;

    /// <summary>
    /// Gets the current emotion
    /// </summary>
    public Models.ChackEmotion CurrentEmotion
    {
        get
        {
            lock (_lock)
            {
                return _currentEmotion;
            }
        }
    }

    /// <summary>
    /// Sets ChackGPT's emotion and notifies all subscribers
    /// </summary>
    /// <param name="emotion">The new emotion to display</param>
    public void SetEmotion(Models.ChackEmotion emotion)
    {
        lock (_lock)
        {
            if (_currentEmotion != emotion)
            {
                var oldEmotion = _currentEmotion;
                _currentEmotion = emotion;
                logger.LogInformation("💫 Emotion changed from {OldEmotion} to {NewEmotion}", oldEmotion, emotion);
                EmotionChanged?.Invoke(this, emotion);
                logger.LogInformation("📢 EmotionChanged event fired to all subscribers");
            }
            else
            {
                logger.LogDebug("Emotion already set to {Emotion}, no change needed", emotion);
            }
        }
    }
}
