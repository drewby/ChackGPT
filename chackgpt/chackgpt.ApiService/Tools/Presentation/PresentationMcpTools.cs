using System.ComponentModel;
using chackgpt.ApiService.Tools.Presentation.Services;
using ModelContextProtocol.Server;

namespace chackgpt.ApiService.Tools.Presentation;

/// <summary>
/// MCP tools for presentation content delivery.
/// Exposes presentation slide retrieval via Model Context Protocol.
/// Reference: https://learn.microsoft.com/en-us/azure/app-service/tutorial-ai-model-context-protocol-server-dotnet
/// </summary>
[McpServerToolType]
public class PresentationMcpTools
{
    private readonly PresentationService _presentationService;
    private readonly ILogger<PresentationMcpTools> _logger;

    public PresentationMcpTools(
        PresentationService presentationService,
        ILogger<PresentationMcpTools> logger)
    {
        _presentationService = presentationService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a specific presentation slide by topic, slide number, and language.
    /// MCP Tool: GetPresentationSlide
    /// </summary>
    /// <param name="topic">The presentation topic</param>
    /// <param name="slideNumber">Slide number (1-based index)</param>
    /// <param name="language">The presentation language (English or Japanese)</param>
    /// <returns>JSON representation of the presentation slide</returns>
    [McpServerTool]
    [Description("Retrieves a presentation slide for the specified topic, slide number, and language. " +
        "Supported topics: Dotnet10 (.NET 10), Intelligence (Microsoft Agent Framework, MCP), Aspire13 (Aspire 13), " +
        "Dotnet10platform (ASP.NET Core 10, .NET Libraries 10, SDK & Tooling 10), Summary (Intelligent Apps Ecosystem). " +
        "Supported languages: Japanese (jp) only. " +
        "Returns structured content with title, description, bullets, and slide navigation info.")]
    public async Task<string> GetPresentationSlide(
        [Description("Presentation topic")] Models.PresentationTopic topic,
        [Description("Slide number (1-based)")] int slideNumber,
        [Description("Presentation language (only Japanese)")] Models.PresentationLanguage language = Models.PresentationLanguage.Japanese)
    {
        try
        {
            _logger.LogInformation("MCP Tool GetPresentationSlide invoked: topic={Topic}, slideNumber={SlideNumber}, language={Language}",
                topic, slideNumber, language);

            var slide = _presentationService.GetSlide(topic, slideNumber, language);

            if (slide == null)
            {
                var totalSlides = _presentationService.GetSlideCount(topic, language);
                var error = $"Slide {slideNumber} not found for topic '{topic}' in language '{language}'. Valid range: 1-{totalSlides}";
                _logger.LogWarning(error);
                return System.Text.Json.JsonSerializer.Serialize(new { error });
            }

            var json = System.Text.Json.JsonSerializer.Serialize(slide);
            _logger.LogInformation("MCP Tool GetPresentationSlide completed successfully");
            
            return await Task.FromResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPresentationSlide MCP tool");
            return System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lists all available presentation topics.
    /// MCP Tool: GetAvailableTopics
    /// </summary>
    /// <returns>JSON array of available topic identifiers</returns>
    [McpServerTool]
    [Description("Lists all available presentation topics. Returns an array of topic identifiers.")]
    public async Task<string> GetAvailableTopics()
    {
        try
        {
            _logger.LogInformation("MCP Tool GetAvailableTopics invoked");

            var topics = _presentationService.GetAvailableTopics();
            var json = System.Text.Json.JsonSerializer.Serialize(topics);

            _logger.LogInformation("MCP Tool GetAvailableTopics completed successfully");
            
            return await Task.FromResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAvailableTopics MCP tool");
            return System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message });
        }
    }
}
