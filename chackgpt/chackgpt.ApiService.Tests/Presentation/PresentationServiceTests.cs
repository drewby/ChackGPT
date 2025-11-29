using chackgpt.ApiService.Tools.Presentation.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace chackgpt.ApiService.Tests.Presentation;

public class PresentationServiceTests
{
    private readonly PresentationService _service;
    private readonly Mock<ILogger<PresentationService>> _loggerMock;

    public PresentationServiceTests()
    {
        _loggerMock = new Mock<ILogger<PresentationService>>();
        _service = new PresentationService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("dotnet10", 1, "en")]
    [InlineData("dotnet10", 2, "en")]
    [InlineData("dotnet10", 4, "en")] // C# 14 is now slide 4
    [InlineData("intelligence", 1, "en")]
    [InlineData("intelligence", 4, "en")]
    [InlineData("aspire13", 1, "en")]
    [InlineData("dotnet10platform", 1, "en")]
    [InlineData("dotnet10platform", 2, "en")]
    [InlineData("dotnet10platform", 3, "en")]
    [InlineData("summary", 1, "en")]
    [InlineData("summary", 1, "jp")]
    [InlineData("dotnet10", 1, "jp")]
    [InlineData("dotnet10", 4, "jp")] // C# 14 is now slide 4
    [InlineData("intelligence", 1, "jp")]
    public void GetSlide_ValidTopicAndSlideNumber_ReturnsSlide(string topic, int slideNumber, string language)
    {
        // Act
        var slide = _service.GetSlide(topic, slideNumber, language);

        // Assert
        Assert.NotNull(slide);
        Assert.Equal(topic, slide.Topic);
        Assert.Equal(slideNumber, slide.CurrentSlideNumber);
        Assert.NotEmpty(slide.Title);
        Assert.NotEmpty(slide.Description);
        Assert.NotEmpty(slide.Bullets);
        Assert.InRange(slide.Bullets.Count, 4, 5);
        Assert.True(slide.TotalSlides >= slideNumber);
    }

    [Theory]
    [InlineData("dotnet10", 0, "en")]
    [InlineData("dotnet10", 999, "en")]
    [InlineData("dotnet10platform", 10, "en")]
    [InlineData("dotnet10", 0, "jp")]
    [InlineData("dotnet10", 999, "jp")]
    public void GetSlide_InvalidSlideNumber_ReturnsNull(string topic, int slideNumber, string language)
    {
        // Act
        var slide = _service.GetSlide(topic, slideNumber, language);

        // Assert
        Assert.Null(slide);
    }

    [Theory]
    [InlineData("invalidtopic", 1, "en")]
    [InlineData("nonexistent", 1, "en")]
    [InlineData("dotnet10", 1, "invalid")]
    public void GetSlide_InvalidTopicOrLanguage_ReturnsNull(string topic, int slideNumber, string language)
    {
        // Act
        var slide = _service.GetSlide(topic, slideNumber, language);

        // Assert
        Assert.Null(slide);
    }

    [Fact]
    public void GetAvailableTopics_ReturnsAllTopics()
    {
        // Act
        var topics = _service.GetAvailableTopics().ToList();

        // Assert
        Assert.NotEmpty(topics);
        Assert.Contains("dotnet10", topics);
        Assert.Contains("intelligence", topics);
        Assert.Contains("aspire13", topics);
        Assert.Contains("dotnet10platform", topics);
        Assert.Contains("summary", topics);
    }

    [Theory]
    [InlineData("dotnet10", 4, "en")] // Now has 4 slides (added C# 14)
    [InlineData("intelligence", 4, "en")]
    [InlineData("aspire13", 2, "en")]
    [InlineData("dotnet10platform", 3, "en")]
    [InlineData("summary", 1, "en")]
    [InlineData("summary", 1, "jp")]
    [InlineData("dotnet10", 4, "jp")] // Now has 4 slides (added C# 14)
    [InlineData("intelligence", 4, "jp")]
    [InlineData("aspire13", 2, "jp")]
    [InlineData("dotnet10platform", 3, "jp")]
    public void GetSlideCount_ValidTopic_ReturnsCorrectCount(string topic, int expectedCount, string language)
    {
        // Act
        var count = _service.GetSlideCount(topic, language);

        // Assert
        Assert.Equal(expectedCount, count);
    }

    [Theory]
    [InlineData("invalidtopic", "en")]
    [InlineData("dotnet10", "invalid")]
    public void GetSlideCount_InvalidTopicOrLanguage_ReturnsZero(string topic, string language)
    {
        // Act
        var count = _service.GetSlideCount(topic, language);

        // Assert
        Assert.Equal(0, count);
    }

    [Theory]
    [InlineData("DOTNET10", "en")]
    [InlineData("DotNet10", "en")]
    [InlineData("dotnet10", "en")]
    [InlineData("dotnet10", "EN")]
    [InlineData("dotnet10", "JP")]
    public void GetSlide_CaseInsensitiveTopicAndLanguage_ReturnsSlide(string topic, string language)
    {
        // Act
        var slide = _service.GetSlide(topic, 1, language);

        // Assert
        Assert.NotNull(slide);
        Assert.Equal("dotnet10", slide.Topic);
    }
}
