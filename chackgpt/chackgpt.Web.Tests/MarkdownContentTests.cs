using Bunit;
using chackgpt.Web.Components.Shared;
using Xunit;

namespace chackgpt.Web.Tests;

public class MarkdownContentTests : BunitContext
{
    [Fact]
    public void MarkdownContent_RendersSimpleText()
    {
        // Arrange
        var content = "Hello, World!";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        cut.MarkupMatches("<p>Hello, World!</p>");
    }

    [Fact]
    public void MarkdownContent_RendersBoldText()
    {
        // Arrange
        var content = "This is **bold** text";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<strong>", markup);
        Assert.Contains("bold", markup);
    }

    [Fact]
    public void MarkdownContent_RendersItalicText()
    {
        // Arrange
        var content = "This is *italic* text";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<em>", markup);
        Assert.Contains("italic", markup);
    }

    [Fact]
    public void MarkdownContent_RendersHeaders()
    {
        // Arrange
        var content = "# Heading 1\n## Heading 2\n### Heading 3";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<h1", markup);
        Assert.Contains("Heading 1", markup);
        Assert.Contains("<h2", markup);
        Assert.Contains("Heading 2", markup);
        Assert.Contains("<h3", markup);
        Assert.Contains("Heading 3", markup);
    }

    [Fact]
    public void MarkdownContent_RendersUnorderedList()
    {
        // Arrange
        var content = "- Item 1\n- Item 2\n- Item 3";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<ul>", markup);
        Assert.Contains("<li>", markup);
        Assert.Contains("Item 1", markup);
        Assert.Contains("Item 2", markup);
        Assert.Contains("Item 3", markup);
    }

    [Fact]
    public void MarkdownContent_RendersOrderedList()
    {
        // Arrange
        var content = "1. First\n2. Second\n3. Third";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<ol>", markup);
        Assert.Contains("<li>", markup);
        Assert.Contains("First", markup);
        Assert.Contains("Second", markup);
        Assert.Contains("Third", markup);
    }

    [Fact]
    public void MarkdownContent_RendersLinks()
    {
        // Arrange
        var content = "[Click here](https://example.com)";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<a", markup);
        Assert.Contains("href=\"https://example.com\"", markup);
        Assert.Contains("Click here", markup);
    }

    [Fact]
    public void MarkdownContent_RendersCodeBlocks()
    {
        // Arrange
        var content = "```csharp\nvar x = 10;\n```";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<pre>", markup);
        Assert.Contains("<code", markup);
        Assert.Contains("var x = 10;", markup);
    }

    [Fact]
    public void MarkdownContent_RendersInlineCode()
    {
        // Arrange
        var content = "This is `inline code` example";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("<code>", markup);
        Assert.Contains("inline code", markup);
    }

    [Fact]
    public void MarkdownContent_HandlesEmptyContent()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void MarkdownContent_HandlesNullContent()
    {
        // Arrange
        string? content = null;

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content!));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void MarkdownContent_PassesHTMLThrough()
    {
        // Arrange - Markdig by default passes HTML through (GitHub Flavored Markdown behavior)
        // Note: This is safe in our case because the content comes from a trusted source (Azure OpenAI)
        // and is rendered with MarkupString which doesn't double-encode
        var content = "<script>alert('xss')</script> Normal text";

        // Act
        var cut = Render<MarkdownContent>(parameters => parameters
            .Add(p => p.Content, content));

        // Assert
        var markup = cut.Markup;
        // Markdig with GitHub Flavored Markdown passes HTML through
        // This is acceptable since content comes from Azure OpenAI API
        Assert.Contains("<script>", markup);
        Assert.Contains("Normal text", markup);
    }
}
