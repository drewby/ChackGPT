namespace chackgpt.Web.Services;

/// <summary>
/// Interface for emotion management services that track and broadcast emotional state changes.
/// </summary>
/// <typeparam name="TEmotion">The emotion enum type (ChackEmotion or DrewEmotion).</typeparam>
public interface IEmotionService<TEmotion> where TEmotion : Enum
{
    /// <summary>
    /// Event raised when the emotion state changes.
    /// </summary>
    event EventHandler<TEmotion>? EmotionChanged;

    /// <summary>
    /// Gets the current emotion state.
    /// </summary>
    TEmotion CurrentEmotion { get; }

    /// <summary>
    /// Sets the emotion state and notifies all subscribers.
    /// </summary>
    /// <param name="emotion">The new emotion to display.</param>
    void SetEmotion(TEmotion emotion);
}
