using System.Text.Json;
using chackgpt.ApiService.Tools.Presentation.Models;

namespace chackgpt.ApiService.Tools.Presentation.Services;

/// <summary>
/// Service for managing and retrieving presentation slide content.
/// Supports the Model Context Protocol (MCP) for .NET presentations.
/// </summary>
public class PresentationService
{
    private readonly Dictionary<string, Dictionary<string, PresentationDeck>> _decksByLanguage;
    private readonly ILogger<PresentationService> _logger;

    public PresentationService(ILogger<PresentationService> logger)
    {
        _logger = logger;
        _decksByLanguage = LoadAllPresentationContent();
    }

    /// <summary>
    /// Gets a specific slide by topic, slide number, and language.
    /// </summary>
    /// <param name="topic">The presentation topic</param>
    /// <param name="slideNumber">The slide number (1-based)</param>
    /// <param name="language">The presentation language (defaults to English)</param>
    /// <returns>The requested slide, or null if not found</returns>
    public PresentationSlide? GetSlide(PresentationTopic topic, int slideNumber, PresentationLanguage language = PresentationLanguage.Japanese)
    {
        return GetSlide(topic.ToTopicId(), slideNumber, language.ToLanguageCode());
    }

    /// <summary>
    /// Gets a specific slide by topic, slide number, and language.
    /// </summary>
    /// <param name="topic">The topic identifier (e.g., "dotnet10", "csharp14", "aspire13", "aspnet10", "dotnetlib10", "sdktooling10")</param>
    /// <param name="slideNumber">The slide number (1-based)</param>
    /// <param name="languageCode">The language code (e.g., "jp")</param>
    /// <returns>The requested slide, or null if not found</returns>
    public PresentationSlide? GetSlide(string topic, int slideNumber, string languageCode = "jp")
    {
        if (!_decksByLanguage.TryGetValue(languageCode.ToLowerInvariant(), out var decks))
        {
            _logger.LogWarning("Language '{Language}' not found", languageCode);
            return null;
        }

        if (!decks.TryGetValue(topic.ToLowerInvariant(), out var deck))
        {
            _logger.LogWarning("Topic '{Topic}' not found for language '{Language}'", topic, languageCode);
            return null;
        }

        if (slideNumber < 1 || slideNumber > deck.Slides.Count)
        {
            _logger.LogWarning("Slide number {SlideNumber} is out of range for topic '{Topic}' (1-{Max})", 
                slideNumber, topic, deck.Slides.Count);
            return null;
        }

        return deck.Slides[slideNumber - 1];
    }

    /// <summary>
    /// Gets all available topic identifiers.
    /// </summary>
    public IEnumerable<string> GetAvailableTopics()
    {
        // Return topics from Japanese content as the default
        return _decksByLanguage.TryGetValue("jp", out var decks) ? decks.Keys : Enumerable.Empty<string>();
    }

    /// <summary>
    /// Gets the total number of slides for a specific topic and language.
    /// </summary>
    public int GetSlideCount(PresentationTopic topic, PresentationLanguage language = PresentationLanguage.Japanese)
    {
        return GetSlideCount(topic.ToTopicId(), language.ToLanguageCode());
    }

    /// <summary>
    /// Gets the total number of slides for a specific topic and language.
    /// </summary>
    public int GetSlideCount(string topic, string languageCode = "jp")
    {
        if (_decksByLanguage.TryGetValue(languageCode.ToLowerInvariant(), out var decks) &&
            decks.TryGetValue(topic.ToLowerInvariant(), out var deck))
        {
            return deck.Slides.Count;
        }
        return 0;
    }

    private Dictionary<string, Dictionary<string, PresentationDeck>> LoadAllPresentationContent()
    {
        var result = new Dictionary<string, Dictionary<string, PresentationDeck>>();
        
        // Load Japanese content
        var japaneseDecks = LoadPresentationContent(PresentationLanguage.Japanese);
        if (japaneseDecks.Count > 0)
        {
            result["jp"] = japaneseDecks;
        }

        return result;
    }

    private Dictionary<string, PresentationDeck> LoadPresentationContent(PresentationLanguage language)
    {
        try
        {
            var fileSuffix = language.ToFileSuffix();
            var fileName = $"presentation-content{fileSuffix}.json";
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Tools", "Presentation", "Data", fileName);
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Presentation content file not found at: {Path}", jsonPath);
                return new Dictionary<string, PresentationDeck>();
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var root = JsonSerializer.Deserialize<PresentationContentRoot>(jsonContent, options);
            
            if (root?.Decks == null)
            {
                _logger.LogError("Failed to deserialize presentation content or no decks found for language {Language}", language);
                return new Dictionary<string, PresentationDeck>();
            }

            var decks = root.Decks.ToDictionary(
                deck => deck.Topic.ToLowerInvariant(),
                deck => deck
            );

            _logger.LogInformation("Loaded {Count} presentation decks for language {Language}: {Topics}", 
                decks.Count, language, string.Join(", ", decks.Keys));

            return decks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading presentation content for language {Language}", language);
            return new Dictionary<string, PresentationDeck>();
        }
    }

    private class PresentationContentRoot
    {
        public List<PresentationDeck> Decks { get; init; } = [];
    }
}
