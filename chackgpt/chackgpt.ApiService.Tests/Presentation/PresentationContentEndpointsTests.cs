using System.Net;
using System.Net.Http.Json;
using chackgpt.ApiService.Tools.Presentation.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace chackgpt.ApiService.Tests.Presentation;

public class PresentationContentEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PresentationContentEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("dotnet10", 1, null)] // Default to English
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
    public async Task GetPresentationSlide_ValidRequest_ReturnsOk(string topic, int slideNumber, string? language)
    {
        // Act
        var url = language == null 
            ? $"/api/presentation/slide/{topic}/{slideNumber}"
            : $"/api/presentation/slide/{topic}/{slideNumber}?language={language}";
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var slide = await response.Content.ReadFromJsonAsync<PresentationSlide>();
        Assert.NotNull(slide);
        Assert.Equal(topic, slide.Topic);
        Assert.Equal(slideNumber, slide.CurrentSlideNumber);
        Assert.NotEmpty(slide.Title);
        Assert.NotEmpty(slide.Description);
        Assert.NotEmpty(slide.Bullets);
        Assert.InRange(slide.Bullets.Count, 4, 5);
    }

    [Theory]
    [InlineData("dotnet10", 0, "en")]
    [InlineData("dotnet10", 999, "en")]
    [InlineData("dotnet10platform", 10, "en")]
    [InlineData("dotnet10", 999, "jp")]
    public async Task GetPresentationSlide_InvalidSlideNumber_ReturnsNotFound(string topic, int slideNumber, string language)
    {
        // Act
        var response = await _client.GetAsync($"/api/presentation/slide/{topic}/{slideNumber}?language={language}");

        // Assert
        if (slideNumber < 1)
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Theory]
    [InlineData("invalidtopic", 1, "en")]
    [InlineData("nonexistent", 1, "en")]
    [InlineData("dotnet10", 1, "invalid")]
    public async Task GetPresentationSlide_InvalidTopicOrLanguage_ReturnsBadRequest(string topic, int slideNumber, string language)
    {
        // Act
        var response = await _client.GetAsync($"/api/presentation/slide/{topic}/{slideNumber}?language={language}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAvailableTopics_ReturnsOkWithTopics()
    {
        // Act
        var response = await _client.GetAsync("/api/presentation/topics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var topics = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.NotNull(topics);
        Assert.NotEmpty(topics);
        Assert.Contains("dotnet10", topics);
        Assert.Contains("intelligence", topics);
        Assert.Contains("aspire13", topics);
        Assert.Contains("dotnet10platform", topics);
        Assert.Contains("summary", topics);
    }

    [Theory]
    [InlineData("DOTNET10", 1, "en")]
    [InlineData("DotNet10", 1, "en")]
    [InlineData("dotnet10", 1, "EN")]
    [InlineData("dotnet10", 1, "JP")]
    public async Task GetPresentationSlide_CaseInsensitiveTopicAndLanguage_ReturnsOk(string topic, int slideNumber, string language)
    {
        // Act
        var response = await _client.GetAsync($"/api/presentation/slide/{topic}/{slideNumber}?language={language}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var slide = await response.Content.ReadFromJsonAsync<PresentationSlide>();
        Assert.NotNull(slide);
        Assert.Equal("dotnet10", slide.Topic);
    }

    [Fact]
    public async Task GetPresentationSlide_Aspire13_HasThreeSlides()
    {
        // Act & Assert for slide 1
        var response1 = await _client.GetAsync("/api/presentation/slide/aspire13/1?language=en");
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        var slide1 = await response1.Content.ReadFromJsonAsync<PresentationSlide>();
        Assert.NotNull(slide1);
        Assert.Equal(3, slide1.TotalSlides);

        // Act & Assert for slide 2
        var response2 = await _client.GetAsync("/api/presentation/slide/aspire13/2?language=en");
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var slide2 = await response2.Content.ReadFromJsonAsync<PresentationSlide>();
        Assert.NotNull(slide2);
        Assert.Equal(3, slide2.TotalSlides);
        Assert.Equal(2, slide2.CurrentSlideNumber);

        // Act & Assert for slide 3
        var response3 = await _client.GetAsync("/api/presentation/slide/aspire13/3?language=en");
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
        var slide3 = await response3.Content.ReadFromJsonAsync<PresentationSlide>();
        Assert.NotNull(slide3);
        Assert.Equal(3, slide3.TotalSlides);
        Assert.Equal(3, slide3.CurrentSlideNumber);

        // Act & Assert for slide 4 (should not exist)
        var response4 = await _client.GetAsync("/api/presentation/slide/aspire13/4?language=en");
        Assert.Equal(HttpStatusCode.NotFound, response4.StatusCode);
    }
}
