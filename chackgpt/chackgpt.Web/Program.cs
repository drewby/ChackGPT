using Azure.AI.OpenAI;
using chackgpt.Web;
using chackgpt.Web.Components;
using chackgpt.Web.Configuration;
using chackgpt.Web.Constants;
using chackgpt.Web.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();
builder.Services.AddFluentUIComponents();

// =============================================================================
// Configuration - .NET 10 Options pattern with DataAnnotations validation
// =============================================================================
// The Options pattern provides strong typing and validation for configuration.
// DataAnnotations attributes on option classes enable automatic validation.

// Configure MCP client options (Model Context Protocol for ApiService communication)
builder.Services.Configure<McpClientOptions>(options =>
{
    options.ApiServiceBaseUrl = "https+http://apiservice"; // Aspire service discovery URL
    options.TimeoutSeconds = 30;
});

// Configure Azure OpenAI options from appsettings.json with automatic validation
builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));

// =============================================================================
// Service Registration - Using interfaces for dependency inversion (DIP)
// =============================================================================
// All services are registered via their interface types for loose coupling.
// This follows the Dependency Inversion Principle: depend on abstractions,
// not concretions. Benefits:
// - Easier unit testing with mocks
// - Flexible implementation swapping
// - Clear contracts between components

builder.Services.AddSingleton<AgentFactory>();

// Model Context Protocol (MCP) client for ApiService communication
builder.Services.AddSingleton<IMcpClientService, McpClientService>();

// ChackGPT emotion service - manages ChackGPT's emotional state
// Uses generic IEmotionService<ChackEmotion> interface for type safety
builder.Services.AddSingleton<IEmotionService<chackgpt.Web.Models.ChackEmotion>, ChackEmotionService>();

// DrewGPT emotion service - manages DrewGPT's emotional state
// Uses generic IEmotionService<DrewEmotion> interface for type safety
builder.Services.AddSingleton<IEmotionService<chackgpt.Web.Models.DrewEmotion>, DrewEmotionService>();

// Slide display service - manages presentation slide display events
builder.Services.AddSingleton<ISlideService, DisplaySlideService>();

// Video display service - manages video playback events
builder.Services.AddSingleton<IVideoService, DisplayVideoService>();

// Chat message coordination service - coordinates messages between UI components
builder.Services.AddSingleton<IChatMessageService, ChatMessageService>();

// .NET 10 IHostedService - Broadcast services with automatic lifecycle management
// These services automatically start when the application starts and stop when it shuts down.
// They listen to domain events and broadcast them to SignalR clients in real-time.
// All use the generic BroadcastHostedService<TEventSource, THub> base class pattern.
builder.Services.AddHostedService<EmotionBroadcastService>();      // ChackGPT emotions
builder.Services.AddHostedService<DrewEmotionBroadcastService>();  // DrewGPT emotions
builder.Services.AddHostedService<SlideBroadcastService>();        // Presentation slides
builder.Services.AddHostedService<VideoBroadcastService>();        // Video playback

// =============================================================================
// Agent Creation and Workflow Configuration
// =============================================================================
// Creates AI agents using the unified AgentFactory and configures a multi-agent
// group chat workflow. Demonstrates .NET 10 AI Agents SDK capabilities:
// - Agent tool registration (C# 14 params collections)
// - Group chat workflows with custom termination logic
// - Agent handoff based on conversation keywords

builder.Services.AddSingleton<Workflow>(sp =>
{
    // Resolve required services from dependency injection (using interfaces for loose coupling)
    IMcpClientService mcpClient = sp.GetRequiredService<IMcpClientService>();
    IEmotionService<chackgpt.Web.Models.ChackEmotion> chackEmotionService = sp.GetRequiredService<IEmotionService<chackgpt.Web.Models.ChackEmotion>>();
    IEmotionService<chackgpt.Web.Models.DrewEmotion> drewEmotionService = sp.GetRequiredService<IEmotionService<chackgpt.Web.Models.DrewEmotion>>();
    ISlideService displaySlideService = sp.GetRequiredService<ISlideService>();
    IVideoService displayVideoService = sp.GetRequiredService<IVideoService>();
    AgentFactory agentFactory = sp.GetRequiredService<AgentFactory>();
    ILogger<Program> logger = sp.GetRequiredService<ILogger<Program>>();
    
    // Gather MCP tools from ApiService (C# demos exposed as AI tools)
    var mcpTools = mcpClient.ListToolsAsync().GetAwaiter().GetResult();
    
    // Create custom AI tools for ChackGPT (presentation-focused agent)
    AITool chackEmotionTool = chackgpt.Web.Tools.ChackEmotionTool.CreateTool(chackEmotionService, logger);
    AITool displaySlideTool = chackgpt.Web.Tools.DisplaySlideTool.CreateTool(displaySlideService, logger);
    AITool displayVideoTool = chackgpt.Web.Tools.DisplayVideoTool.CreateTool(displayVideoService, logger);
    
    // Combine MCP tools and custom tools for ChackGPT
    List<AITool> chackTools = new(mcpTools.Count + 3);
    chackTools.AddRange(mcpTools);      // C# 14 demos from ApiService
    chackTools.Add(chackEmotionTool);   // Emotion display capability
    chackTools.Add(displaySlideTool);   // Slide presentation capability
    chackTools.Add(displayVideoTool);   // Video playback capability
    
    // Create ChackGPT agent with all tools (uses C# 14 params collections and collection expressions)
    AIAgent chackAgent = agentFactory.CreateAgent(
        "ChackGPT",
        ChatConstants.SystemMessage,
        [.. chackTools]); // C# 14 collection expression
    
    // Create DrewGPT agent with emotion tool only (simpler agent for demos)
    AITool drewEmotionTool = chackgpt.Web.Tools.DrewEmotionTool.CreateTool(drewEmotionService, logger);
    
    AIAgent drewAgent = agentFactory.CreateAgent(
        "DrewGPT",
        ChatConstants.DrewSystemMessage,
        drewEmotionTool); // C# 14 params collections - single tool
    
    // Build group chat workflow with custom AspireKeywordGroupChatManager
    // This workflow enables multi-agent conversation where agents take turns.
    // The custom manager enforces that "aspire" keyword must be mentioned,
    // demonstrating controlled agent handoff and conversation flow.
    Workflow workflow = AgentWorkflowBuilder
        .CreateGroupChatBuilderWith(agents => 
            new AspireKeywordGroupChatManager(agents)
            {
                MaximumIterationCount = 5  // Limit conversation to 5 turns max
            })
        .AddParticipants(chackAgent, drewAgent)  // Two-agent conversation
        .Build();
    
    logger.LogInformation("✅ Created group chat workflow with ChackGPT and DrewGPT (Aspire keyword required for handoff)");
    
    return workflow;
});

builder.Services.AddSignalR();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

// =============================================================================
// Application Pipeline Configuration
// =============================================================================
// Configure the HTTP request pipeline with middleware components.
// Order matters: middleware executes in the order added.

var app = builder.Build();

// Broadcast services are IHostedService - they start automatically with the app

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseOutputCache();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
app.MapHub<chackgpt.Web.Hubs.ChatHub>("/chathub");

// =============================================================================
// Custom Endpoints
// =============================================================================

// Video streaming endpoint with HTTP range request support
// Enables video seeking, pause/resume, and efficient streaming.
// Demonstrates .NET 10 minimal API with advanced file serving capabilities.
app.MapGet("/videos/{filename}", async (string filename, HttpContext context) =>
{
    var videoPath = Path.Combine(builder.Environment.WebRootPath, "videos", filename);
    
    if (!File.Exists(videoPath))
    {
        return Results.NotFound();
    }
    
    // Disable response buffering to enable true streaming (not loading entire file)
    var bufferingFeature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseBodyFeature>();
    if (bufferingFeature != null)
    {
        bufferingFeature.DisableBuffering();
    }
    
    // Return file with range processing enabled for video player controls
    return Results.File(
        videoPath,
        contentType: "video/mp4",
        enableRangeProcessing: true); // Enables HTTP 206 Partial Content for seeking
})
.WithName("GetVideo")
.WithDescription("Streams video files with HTTP range request support for seeking and resuming");

app.Run();
