using chackgpt.ApiService.Tools.Video.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace chackgpt.ApiService.Tests.Video;

public class VideoServiceTests
{
    private readonly VideoService _service;
    private readonly Mock<ILogger<VideoService>> _loggerMock;

    public VideoServiceTests()
    {
        _loggerMock = new Mock<ILogger<VideoService>>();
        _service = new VideoService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("what-will-happen-to-chack")]
    [InlineData("dotnet10-overview")]
    [InlineData("csharp14-features")]
    [InlineData("aspnet-core-10")]
    [InlineData("aspire-cloud-native")]
    [InlineData("post-quantum-crypto")]
    public void GetVideoById_ValidId_ReturnsVideo(string videoId)
    {
        // Act
        var video = _service.GetVideoById(videoId);

        // Assert
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
    public void GetVideoByTitle_ValidTitle_ReturnsVideo(string title)
    {
        // Act
        var video = _service.GetVideoByTitle(title);

        // Assert
        Assert.NotNull(video);
        Assert.Equal(title, video.Title);
        Assert.NotEmpty(video.Id);
        Assert.NotEmpty(video.Description);
        Assert.NotEmpty(video.VideoUrl);
    }

    [Theory]
    [InlineData("nonexistent-video")]
    [InlineData("invalid-id")]
    public void GetVideoById_InvalidId_ReturnsNull(string videoId)
    {
        // Act
        var video = _service.GetVideoById(videoId);

        // Assert
        Assert.Null(video);
    }

    [Fact]
    public void GetVideoByTitle_InvalidTitle_ReturnsNull()
    {
        // Act
        var video = _service.GetVideoByTitle("Nonexistent Video Title");

        // Assert
        Assert.Null(video);
    }

    [Fact]
    public void GetAllVideos_ReturnsAllVideos()
    {
        // Act
        var videos = _service.GetAllVideos().ToList();

        // Assert
        Assert.NotEmpty(videos);
        Assert.Equal(6, videos.Count);
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
    [InlineData("what-will-happen-to-chack")]
    public void GetVideoById_CaseInsensitiveId_ReturnsVideo(string videoId)
    {
        // Act
        var video = _service.GetVideoById(videoId);

        // Assert
        Assert.NotNull(video);
        Assert.Equal("what-will-happen-to-chack", video.Id);
    }

    [Theory]
    [InlineData("WHAT WILL HAPPEN TO CHACK?")]
    [InlineData("What Will Happen To Chack?")]
    [InlineData("what will happen to chack?")]
    public void GetVideoByTitle_CaseInsensitiveTitle_ReturnsVideo(string title)
    {
        // Act
        var video = _service.GetVideoByTitle(title);

        // Assert
        Assert.NotNull(video);
        Assert.Equal("What will happen to Chack?", video.Title);
    }
}
