namespace chackgpt.Web.Services;

/// <summary>
/// Interface for chat message coordination services across components.
/// </summary>
public interface IChatMessageService
{
    /// <summary>
    /// Event raised when a component wants to send a message to the chat.
    /// </summary>
    event EventHandler<string>? MessageRequested;

    /// <summary>
    /// Event raised when a chat message token is received while popup is visible.
    /// </summary>
    event EventHandler<string>? ChatMessageReceived;

    /// <summary>
    /// Event raised when popup visibility changes (opened or closed).
    /// </summary>
    event EventHandler<bool>? PopupVisibilityChanged;

    /// <summary>
    /// Event raised when the next slide button is clicked.
    /// </summary>
    event EventHandler? NextSlideRequested;

    /// <summary>
    /// Event raised when the streaming/thinking state changes.
    /// </summary>
    event EventHandler<bool>? StreamingStateChanged;

    /// <summary>
    /// Event raised when the current agent changes (ChackGPT or DrewGPT).
    /// </summary>
    event EventHandler<string>? AgentChanged;

    /// <summary>
    /// Gets or sets whether the slide popup is currently visible.
    /// </summary>
    bool IsPopupVisible { get; set; }

    /// <summary>
    /// Gets whether the chat is currently streaming a response.
    /// </summary>
    bool IsStreaming { get; }

    /// <summary>
    /// Request to send a message to the chat.
    /// </summary>
    /// <param name="message">The message to send.</param>
    void RequestMessage(string message);

    /// <summary>
    /// Request to go to the next slide.
    /// </summary>
    void RequestNextSlide();

    /// <summary>
    /// Notify that a chat message token was received.
    /// </summary>
    /// <param name="token">The message token.</param>
    void NotifyChatMessage(string token);

    /// <summary>
    /// Update the streaming/thinking state for listeners.
    /// </summary>
    /// <param name="isStreaming">True when chat is streaming a response.</param>
    void UpdateStreamingState(bool isStreaming);

    /// <summary>
    /// Notify that the current agent has changed.
    /// </summary>
    /// <param name="agentName">The name of the new agent (ChackGPT or DrewGPT).</param>
    void NotifyAgentChange(string agentName);
}
