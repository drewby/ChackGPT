using System.Net;
using System.Net.Http.Json;
using chackgpt.ApiService.Tools.Video.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace chackgpt.ApiService.Tests.Video;

public class VideoContentEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VideoContentEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("what-will-happen-to-chack")]
    [InlineData("dotnet10-overview")]
    [InlineData("csharp14-features")]
    [InlineData("aspnet-core-10")]
    public async Task GetVideo_ValidId_ReturnsOk(string videoId)
    {
        // Act
        var response = await _client.GetAsync($"/api/video/{videoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var video = await response.Content.ReadFromJsonAsync<VideoContent>();
        Assert.NotNull(video);
        Assert.Equal(videoId, video.Id);
        Assert.NotEmpty(video.Title);
        Assert.NotEmpty(video.Description);
        Assert.NotEmpty(video.VideoUrl);
    }

    [Theory]
    [InlineData("What will happen to Chack?")]
    [InlineData(".NET 10 Overview")]
    [InlineData("C# 14 Language Features")]
    public async Task GetVideo_ValidTitle_ReturnsOk(string title)
    {
        // URL encode the title
        var encodedTitle = Uri.EscapeDataString(title);

        // Act
        var response = await _client.GetAsync($"/api/video/{encodedTitle}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var video = await response.Content.ReadFromJsonAsync<VideoContent>();
        Assert.NotNull(video);
        Assert.Equal(title, video.Title);
        Assert.NotEmpty(video.Id);
        Assert.NotEmpty(video.Description);
        Assert.NotEmpty(video.VideoUrl);
    }

    [Theory]
    [InlineData("nonexistent-video")]
    [InlineData("invalid-id")]
    public async Task GetVideo_InvalidIdentifier_ReturnsNotFound(string identifier)
    {
        // Act
        var response = await _client.GetAsync($"/api/video/{identifier}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllVideos_ReturnsOkWithAllVideos()
    {
        // Act
        var response = await _client.GetAsync("/api/video");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var videos = await response.Content.ReadFromJsonAsync<VideoContent[]>();
        Assert.NotNull(videos);
        Assert.Equal(6, videos.Length);
        Assert.All(videos, video =>
        {
            Assert.NotEmpty(video.Id);
            Assert.NotEmpty(video.Title);
            Assert.NotEmpty(video.Description);
            Assert.NotEmpty(video.VideoUrl);
        });
    }

    [Theory]
    [InlineData("WHAT-WILL-HAPPEN-TO-CHACK")]
    [InlineData("What-Will-Happen-To-Chack")]
    public async Task GetVideo_CaseInsensitiveId_ReturnsOk(string videoId)
    {
        // Act
        var response = await _client.GetAsync($"/api/video/{videoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var video = await response.Content.ReadFromJsonAsync<VideoContent>();
        Assert.NotNull(video);
        Assert.Equal("what-will-happen-to-chack", video.Id);
    }

    [Fact]
    public async Task GetVideo_ChackVideo_HasCorrectMetadata()
    {
        // Act
        var response = await _client.GetAsync("/api/video/what-will-happen-to-chack");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var video = await response.Content.ReadFromJsonAsync<VideoContent>();
        Assert.NotNull(video);
        Assert.Equal("what-will-happen-to-chack", video.Id);
        Assert.Equal("What will happen to Chack?", video.Title);
        Assert.Contains("Chack", video.Description);
        Assert.StartsWith("https://", video.VideoUrl);
    }
}
