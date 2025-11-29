namespace chackgpt.Web.Services;

/// <summary>
/// Service to coordinate chat messages across different components
/// </summary>
// C# 14 Primary Constructor - eliminates boilerplate field assignments
public class ChatMessageService(ILogger<ChatMessageService> logger) : IChatMessageService
{
    private bool _isPopupVisible;
    private bool _isStreaming;
    
    /// <summary>
    /// Event raised when a component wants to send a message to the chat
    /// </summary>
    public event EventHandler<string>? MessageRequested;
    
    /// <summary>
    /// Event raised when a chat message token is received while popup is visible
    /// </summary>
    public event EventHandler<string>? ChatMessageReceived;
    
    /// <summary>
    /// Event raised when popup visibility changes (opened or closed)
    /// </summary>
    public event EventHandler<bool>? PopupVisibilityChanged;
    
    /// <summary>
    /// Event raised when the next slide button is clicked
    /// </summary>
    public event EventHandler? NextSlideRequested;
    
    /// <summary>
    /// Event raised when the streaming/thinking state changes
    /// </summary>
    public event EventHandler<bool>? StreamingStateChanged;
    
    /// <summary>
    /// Event raised when the current agent changes (ChackGPT or DrewGPT)
    /// </summary>
    public event EventHandler<string>? AgentChanged;
    
    /// <summary>
    /// Tracks whether the slide popup is currently visible
    /// </summary>
    public bool IsPopupVisible 
    { 
        get => _isPopupVisible;
        set 
        {
            if (_isPopupVisible != value)
            {
                _isPopupVisible = value;
                PopupVisibilityChanged?.Invoke(this, value);
            }
        }
    }

    /// <summary>
    /// Tracks whether the chat is currently streaming a response
    /// </summary>
    public bool IsStreaming
    {
        get => _isStreaming;
        private set
        {
            if (_isStreaming != value)
            {
                _isStreaming = value;
                StreamingStateChanged?.Invoke(this, value);
            }
        }
    }

    /// <summary>
    /// Request to send a message to the chat
    /// </summary>
    /// <param name="message">The message to send</param>
    public void RequestMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message); 
        
        logger.LogInformation("💬 ChatMessageService: Message requested: {Message}", message);
        MessageRequested?.Invoke(this, message);
    }
    
    /// <summary>
    /// Request to go to the next slide
    /// </summary>
    public void RequestNextSlide()
    {
        logger.LogInformation("💬 ChatMessageService: Next slide requested");
        NextSlideRequested?.Invoke(this, EventArgs.Empty);
        RequestMessage("Next Slide");
    }
    
    /// <summary>
    /// Notify that a chat message token was received
    /// </summary>
    /// <param name="token">The message token</param>
    public void NotifyChatMessage(string token)
    {
        ArgumentNullException.ThrowIfNull(token); 
        
        logger.LogDebug("💬 ChatMessageService: Chat message token received");
        ChatMessageReceived?.Invoke(this, token);
    }

    /// <summary>
    /// Update the streaming/thinking state for listeners.
    /// </summary>
    /// <param name="isStreaming">True when chat is streaming a response.</param>
    public void UpdateStreamingState(bool isStreaming)
    {
        logger.LogDebug("💬 ChatMessageService: Streaming state changed to {IsStreaming}", isStreaming);
        IsStreaming = isStreaming;
    }
    
    /// <summary>
    /// Notify that the current agent has changed
    /// </summary>
    /// <param name="agentName">The name of the new agent (ChackGPT or DrewGPT)</param>
    public void NotifyAgentChange(string agentName)
    {
        ArgumentNullException.ThrowIfNull(agentName); 
        
        logger.LogInformation("💬 ChatMessageService: Agent changed to {AgentName}", agentName);
        AgentChanged?.Invoke(this, agentName);
    }
}
