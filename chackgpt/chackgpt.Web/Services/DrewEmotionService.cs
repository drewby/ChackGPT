using Microsoft.Extensions.Logging;

namespace chackgpt.Web.Services;

/// <summary>
/// Service to manage and broadcast DrewGPT's emotional state across the application
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class DrewEmotionService(ILogger<DrewEmotionService> logger) : IEmotionService<Models.DrewEmotion>
{
    private Models.DrewEmotion _currentEmotion = Models.DrewEmotion.Neutral;
    private readonly object _lock = new();
    
    /// <summary>
    /// Event raised when DrewGPT's emotion changes
    /// </summary>
    public event EventHandler<Models.DrewEmotion>? EmotionChanged;

    /// <summary>
    /// Gets the current emotion
    /// </summary>
    public Models.DrewEmotion CurrentEmotion
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
    /// Sets DrewGPT's emotion and notifies all subscribers
    /// </summary>
    /// <param name="emotion">The new emotion to display</param>
    public void SetEmotion(Models.DrewEmotion emotion)
    {
        lock (_lock)
        {
            if (_currentEmotion != emotion)
            {
                var oldEmotion = _currentEmotion;
                _currentEmotion = emotion;
                logger.LogInformation("💫 Drew emotion changed from {OldEmotion} to {NewEmotion}", oldEmotion, emotion);
                EmotionChanged?.Invoke(this, emotion);
                logger.LogInformation("📢 Drew EmotionChanged event fired to all subscribers");
            }
            else
            {
                logger.LogDebug("Drew emotion already set to {Emotion}, no change needed", emotion);
            }
        }
    }
}
