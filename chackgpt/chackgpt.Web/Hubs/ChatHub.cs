using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;

namespace chackgpt.Web.Hubs;

/// <summary>
/// ASP.NET Core 10 SignalR for real-time chat streaming
/// Microsoft Agent Framework for AI conversations with multi-turn conversation support
/// Reference: https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/multi-turn-conversation
/// </summary>
public class ChatHub : Hub
{
    private readonly Workflow _workflow;
    private readonly ILogger<ChatHub> _logger;
    private readonly Services.IEmotionService<Models.ChackEmotion> _emotionService;
    private readonly Services.IEmotionService<Models.DrewEmotion> _drewEmotionService;
    private readonly Services.ISlideService _displaySlideService;
    private readonly Services.IVideoService _displayVideoService;
    private readonly Services.IChatMessageService _chatMessageService;
    
    // Store conversation messages per connection for workflow execution
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, List<ChatMessage>> _conversationMessages = new();
    
    // ActivitySource for distributed tracing of chat operations
    private static readonly ActivitySource _activitySource = new("ChackGPT.ChatHub", "1.0.0");

    public ChatHub(
        Workflow workflow,
        ILogger<ChatHub> logger,
        Services.IEmotionService<Models.ChackEmotion> emotionService,
        Services.IEmotionService<Models.DrewEmotion> drewEmotionService,
        Services.ISlideService displaySlideService,
        Services.IVideoService displayVideoService,
        Services.IChatMessageService chatMessageService)
    {
        _workflow = workflow;
        _logger = logger;
        _emotionService = emotionService;
        _drewEmotionService = drewEmotionService;
        _displaySlideService = displaySlideService;
        _displayVideoService = displayVideoService;
        _chatMessageService = chatMessageService;
    }

    /// <summary>
    /// Called when a client connects - initialize conversation thread and send current emotion and slide state
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("🔌 Client connected to ChatHub: {ConnectionId}", Context.ConnectionId);
        
        // Create new conversation message list for this connection
        List<ChatMessage> messages = new();
        _conversationMessages.TryAdd(Context.ConnectionId, messages);
        _logger.LogInformation("🧵 Created new conversation for {ConnectionId}", Context.ConnectionId);
        
        // Send current emotion to newly connected client (emotion persists as avatar state)
        var currentEmotion = _emotionService.CurrentEmotion.ToString().ToLowerInvariant();
        _logger.LogInformation("📤 Sending current emotion to new client {ConnectionId}: {Emotion}", Context.ConnectionId, currentEmotion);
        await Clients.Caller.SendAsync("EmotionChanged", currentEmotion);
        
        // Send current Drew emotion to newly connected client
        var currentDrewEmotion = _drewEmotionService.CurrentEmotion.ToString().ToLowerInvariant();
        _logger.LogInformation("📤 Sending current Drew emotion to new client {ConnectionId}: {Emotion}", Context.ConnectionId, currentDrewEmotion);
        await Clients.Caller.SendAsync("DrewEmotionChanged", currentDrewEmotion);
        
        // Note: We intentionally do NOT replay slides or videos on reconnection.
        // These are transient UI elements that users can close, and replaying them
        // on every browser refresh creates a poor user experience.
        // If needed, users can ask ChackGPT to show content again.
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects - cleanup conversation thread
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("🔌 Client disconnected from ChatHub: {ConnectionId}", Context.ConnectionId);
        
        // Remove conversation messages for this connection
        _conversationMessages.TryRemove(Context.ConnectionId, out _);
        _logger.LogInformation("🧵 Removed conversation for {ConnectionId}", Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send a message to ChackGPT and stream the response back to the client.
    /// The agent automatically invokes MCP tools (GetPresentationSlide, GetVideo) 
    /// as needed based on the user's request, with tool invocations handled transparently
    /// by the Microsoft Agent Framework.
    /// Uses AgentThread to maintain multi-turn conversation state.
    /// Reference: https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/multi-turn-conversation
    /// </summary>
    /// <param name="userMessage">The user's message</param>
    public async Task SendMessage(string userMessage)
    {
        // Create a distributed trace span for the entire chat message handling
        // This ensures all agent work (including MCP tool calls) are child spans
        using var activity = _activitySource.StartActivity("ChatHub.SendMessage", ActivityKind.Server);
        activity?.SetTag("user.message", userMessage);
        activity?.SetTag("connection.id", Context.ConnectionId);
        
        try
        {
            _logger.LogInformation("Received message from user: {Message}", userMessage);

            // Get conversation messages for this connection
            if (!_conversationMessages.TryGetValue(Context.ConnectionId, out var messages))
            {
                // Create new message list if not found (shouldn't happen normally)
                messages = new List<ChatMessage>();
                _conversationMessages.TryAdd(Context.ConnectionId, messages);
                _logger.LogWarning("🧵 Messages not found for {ConnectionId}, created new list", Context.ConnectionId);
            }

            // Add user message to conversation
            messages.Add(new ChatMessage(ChatRole.User, userMessage));

            // Execute workflow with streaming (using 'await using' to properly dispose the run)
            await using var run = await InProcessExecution.StreamAsync(_workflow, messages);
            
            // Send TurnToken to start workflow execution (required for agent workflows)
            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            
            var tokenCount = 0;
            System.Text.StringBuilder accumulator = new(); 
            var skippingBadText = false;
            string? currentAgentId = null;
            List<ChatMessage>? newMessages = null;
            
            // Patterns that indicate the LLM is hallucinating function call syntax instead of using tools
            var badPatterns = new[]
            {
                "ChackGPT to=functions",
                "{\"emotion\":",
                "to=functions.",
                "DisplaySlide(",
                "SetChackEmotion(",
                "SetDrewEmotion(",
                "GetVideo(",
                "DisplayVideo(",
                "GetPresentationSlide("
            };
            
            await foreach (var evt in run.WatchStreamAsync())
            {
                if (evt is WorkflowOutputEvent output)
                {
                    // Capture the updated message list from the workflow (includes agent responses)
                    newMessages = (List<ChatMessage>)output.Data!;
                    _logger.LogInformation("📝 Workflow completed with {Count} messages", newMessages.Count);
                    break;
                }
                else if (evt is AgentRunUpdateEvent e)
                {
                    // Track which agent is speaking
                    if (currentAgentId != e.ExecutorId)
                    {
                        currentAgentId = e.ExecutorId;
                        var agentName = currentAgentId?.Contains("ChackGPT", StringComparison.OrdinalIgnoreCase) == true ? "ChackGPT" : "DrewGPT";
                        _logger.LogInformation("🎭 Agent switched to: {AgentName}", agentName);
                        
                        // Send agent identifier to all clients (so avatar components can respond)
                        await Clients.All.SendAsync("AgentChanged", agentName);
                        
                        // Notify ChatMessageService so chat bubbles can reset
                        _chatMessageService.NotifyAgentChange(agentName);
                    }
                    
                    var updateText = e.Update?.Text;
                    if (!string.IsNullOrEmpty(updateText))
                    {
                        // Always accumulate when not skipping
                        accumulator.Append(updateText);
                        var accumulated = accumulator.ToString();
                    
                        // If we're skipping bad text, wait for newline to resume
                        if (skippingBadText)
                        {
                            if (accumulated.Contains('\n'))
                            {
                                var newlineIndex = accumulated.IndexOf('\n');
                                var rejectedText = accumulated.Substring(0, newlineIndex);
                                var afterNewline = accumulated.Substring(newlineIndex + 1);
                                _logger.LogInformation("Rejected bad text: {RejectedText}. Resuming with: {Text}", 
                                    rejectedText, afterNewline);
                                accumulator.Clear();
                                skippingBadText = false;
                                
                                if (!string.IsNullOrEmpty(afterNewline))
                                {
                                    await Clients.Caller.SendAsync("ReceiveMessageToken", afterNewline);
                                    tokenCount++;
                                }
                            }
                            continue;
                        }
                        
                        // Check if accumulated text matches the beginning of any bad pattern
                        var trimmed = accumulated.Trim();
                        string? matchingPattern = null;
                        bool isPartialMatch = false;
                        
                        foreach (var pattern in badPatterns)
                        {
                            // Check if the trimmed text could be the start of this pattern
                            if (pattern.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase))
                            {
                                isPartialMatch = true;
                                
                                // Check if we have the full pattern
                                if (trimmed.Length >= pattern.Length)
                                {
                                    matchingPattern = pattern;
                                    break;
                                }
                            }
                            // Check if trimmed text contains the pattern (for cases where pattern appears mid-text)
                            else if (trimmed.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                            {
                                matchingPattern = pattern;
                                break;
                            }
                        }
                        
                        if (matchingPattern != null)
                        {
                            // Full bad pattern confirmed - start skipping
                            _logger.LogWarning("Confirmed bad pattern '{Pattern}' - will skip until newline. Accumulated text: {Text}", 
                                matchingPattern, trimmed);
                            skippingBadText = true;
                            continue;
                        }
                        else if (isPartialMatch)
                        {
                            // Still potentially matching a bad pattern - keep accumulating
                            continue;
                        }
                        else
                        {
                            // No pattern matches - send accumulated text and clear
                            await Clients.Caller.SendAsync("ReceiveMessageToken", accumulated);
                            tokenCount++;
                            accumulator.Clear();
                        }
                    }
                }
            }

            // Signal that the response is complete
            await Clients.Caller.SendAsync("ReceiveMessageComplete");

            // Update conversation history with new messages from workflow (includes agent responses)
            if (newMessages != null && newMessages.Count > messages.Count)
            {
                // Add only the new messages (agent responses) to the conversation history
                var newMessageCount = newMessages.Count - messages.Count;
                messages.AddRange(newMessages.Skip(messages.Count));
                _logger.LogInformation("📝 Added {Count} new agent messages to conversation history", newMessageCount);
            }

            activity?.SetTag("response.token_count", tokenCount);
            activity?.SetStatus(ActivityStatusCode.Ok);
            
            _logger.LogInformation("Successfully completed streaming response for message: {Message}", userMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", userMessage);
            
            // Record exception in the trace
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace);
            
            // Provide more specific error information for MCP-related failures
            var errorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage += $" (Inner: {ex.InnerException.Message})";
                _logger.LogError("Inner exception: {InnerException}", ex.InnerException.Message);
            }
            
            await Clients.Caller.SendAsync("ReceiveError", $"Error: {errorMessage}");
        }

    }
}
