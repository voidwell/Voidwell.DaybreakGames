using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class BBCodeConverterTest
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("plain", "plain")]
    [InlineData("[h1]Title[/h1]", "<h1>Title</h1>")]
    [InlineData("[b]bold[/b] [i]it[/i]", "<strong>bold</strong> <em>it</em>")]
    [InlineData("[hr][/hr]", "<hr />")]
    [InlineData("a\nb", "a<br />b")]
    [InlineData("<script>alert(1)</script>", "&lt;script&gt;alert(1)&lt;/script&gt;")]
    [InlineData("[unknown]x[/unknown]", "[unknown]x[/unknown]")]
    [InlineData("[b]unclosed", "<strong>unclosed</strong>")]
    [InlineData("[noparse][b]x[/b][/noparse]", "[b]x[/b]")]
    public void ToHtml_Basics(string input, string expected)
    {
        Assert.Equal(expected, BBCodeConverter.ToHtml(input));
    }

    [Fact]
    public void ToHtml_ListWithAndWithoutItemCloseTags()
    {
        var html = BBCodeConverter.ToHtml("[list]\n[*][p]One[/p][/*]\n[*]Two\n[*]Three\n[/list]");

        Assert.Equal("<ul><li><p>One</p></li><li>Two</li><li>Three</li></ul>", html);
    }

    [Fact]
    public void ToHtml_ClosingOuterTagClosesInnerTags()
    {
        Assert.Equal("<strong>a<em>b</em></strong>", BBCodeConverter.ToHtml("[b]a[i]b[/b]"));
    }

    [Fact]
    public void ToHtml_UrlWithValueAndWithBody()
    {
        Assert.Equal(
            "<a href=\"https://example.com/\" target=\"_blank\" rel=\"noopener noreferrer\">site</a>",
            BBCodeConverter.ToHtml("[url=https://example.com/]site[/url]"));
        Assert.Equal(
            "<a href=\"https://example.com/a\" target=\"_blank\" rel=\"noopener noreferrer\">https://example.com/a</a>",
            BBCodeConverter.ToHtml("[url]https://example.com/a[/url]"));
    }

    [Fact]
    public void ToHtml_RejectsNonHttpUrls()
    {
        Assert.Equal("click", BBCodeConverter.ToHtml("[url=javascript:alert(1)]click[/url]"));
        Assert.Equal("", BBCodeConverter.ToHtml("[img]javascript:alert(1)[/img]"));
    }

    [Fact]
    public void ToHtml_ImageWithSteamClanImagePlaceholder()
    {
        Assert.Equal(
            "<img src=\"https://clan.fastly.steamstatic.com/images/123/pic.png\" loading=\"lazy\" alt=\"\" />",
            BBCodeConverter.ToHtml("[img]{STEAM_CLAN_IMAGE}/123/pic.png[/img]"));
    }

    [Fact]
    public void ToHtml_YouTubeEmbedValidatesId()
    {
        Assert.Contains("youtube.com/embed/dQw4w9WgXcQ", BBCodeConverter.ToHtml("[previewyoutube=dQw4w9WgXcQ;full][/previewyoutube]"));
        Assert.DoesNotContain("iframe", BBCodeConverter.ToHtml("[previewyoutube=\"><script>;full][/previewyoutube]"));
    }
}
