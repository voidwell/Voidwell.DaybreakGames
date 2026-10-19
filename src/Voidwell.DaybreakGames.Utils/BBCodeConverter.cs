using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Voidwell.DaybreakGames.Utils;

/// <summary>
/// Converts the BBCode dialect used by Steam news posts to sanitized HTML.
/// All text is HTML-encoded, only a fixed set of tags is emitted, and only http(s) URLs are allowed.
/// </summary>
public static partial class BBCodeConverter
{
    private const string _steamClanImagePlaceholder = "{STEAM_CLAN_IMAGE}";
    private const string _steamClanImageBase = "https://clan.fastly.steamstatic.com/images/";

    // Tags that map directly to an HTML element: bbcode name -> html name
    private static readonly Dictionary<string, string> _simpleTags = new(StringComparer.OrdinalIgnoreCase)
    {
        ["h1"] = "h1",
        ["h2"] = "h2",
        ["h3"] = "h3",
        ["p"] = "p",
        ["b"] = "strong",
        ["i"] = "em",
        ["u"] = "u",
        ["s"] = "s",
        ["strike"] = "s",
        ["quote"] = "blockquote",
        ["code"] = "pre",
        ["spoiler"] = "span",
        ["list"] = "ul",
        ["olist"] = "ol",
        ["table"] = "table",
        ["tr"] = "tr",
        ["th"] = "th",
        ["td"] = "td"
    };

    // Containers whose direct text is layout whitespace, so newlines must not become <br>
    private static readonly HashSet<string> _noLineBreakContainers = new(StringComparer.OrdinalIgnoreCase)
    {
        "list", "olist", "table", "tr"
    };

    [GeneratedRegex(@"\[(/?)([a-zA-Z0-9*]+)(?:=([^\]]*))?\]")]
    private static partial Regex TagRegex();

    [GeneratedRegex("^[A-Za-z0-9_-]{6,20}$")]
    private static partial Regex YouTubeIdRegex();

    private sealed record OpenTag(string Name, string CloseHtml);

    public static string ToHtml(string? bbcode)
    {
        if (string.IsNullOrEmpty(bbcode))
        {
            return string.Empty;
        }

        var text = bbcode.Replace("\r\n", "\n");
        var output = new StringBuilder(text.Length * 2);
        var stack = new Stack<OpenTag>();
        var position = 0;

        foreach (Match match in TagRegex().Matches(text))
        {
            if (match.Index < position)
            {
                continue;
            }

            var isClosing = match.Groups[1].Length > 0;
            var name = match.Groups[2].Value.ToLowerInvariant();
            var value = match.Groups[3].Success ? match.Groups[3].Value : null;

            if (!IsKnownTag(name))
            {
                // Unknown tag: leave it in the text so it renders literally
                continue;
            }

            AppendText(output, text[position..match.Index], stack);
            position = match.Index + match.Length;

            if (isClosing)
            {
                CloseTag(output, stack, name);
                continue;
            }

            switch (name)
            {
                case "hr":
                    output.Append("<hr />");
                    break;

                case "*":
                    // List items often have no explicit close tag, so a new item closes the previous one
                    if (stack.TryPeek(out var top) && top.Name == "li")
                    {
                        output.Append(stack.Pop().CloseHtml);
                    }
                    output.Append("<li>");
                    stack.Push(new OpenTag("li", "</li>"));
                    break;

                case "noparse":
                    position = AppendRaw(output, text, position, "noparse", encodeOnly: true, out _);
                    break;

                case "img":
                    position = AppendRaw(output, text, position, "img", encodeOnly: false, out _);
                    break;

                case "url" when value == null:
                    position = AppendRaw(output, text, position, "url", encodeOnly: false, out _);
                    break;

                case "url":
                    OpenLink(output, stack, value!);
                    break;

                case "previewyoutube":
                    position = AppendRaw(output, text, position, "previewyoutube", encodeOnly: false, out _, value);
                    break;

                default:
                    OpenSimple(output, stack, name, value);
                    break;
            }
        }

        AppendText(output, text[position..], stack);

        while (stack.Count > 0)
        {
            output.Append(stack.Pop().CloseHtml);
        }

        return output.ToString();
    }

    private static bool IsKnownTag(string name)
    {
        return _simpleTags.ContainsKey(name) || name is "hr" or "*" or "noparse" or "img" or "url" or "previewyoutube";
    }

    private static void OpenSimple(StringBuilder output, Stack<OpenTag> stack, string name, string? value)
    {
        var element = _simpleTags[name];
        var attribute = name switch
        {
            "spoiler" => " class=\"bb-spoiler\"",
            "quote" when !string.IsNullOrWhiteSpace(value) => $" data-author=\"{WebUtility.HtmlEncode(value)}\"",
            _ => string.Empty
        };

        output.Append($"<{element}{attribute}>");
        stack.Push(new OpenTag(name, $"</{element}>"));
    }

    private static void OpenLink(StringBuilder output, Stack<OpenTag> stack, string value)
    {
        var href = SanitizeUrl(value);
        if (href == null)
        {
            // Keep the stack balanced but emit nothing for an unsafe link
            stack.Push(new OpenTag("url", string.Empty));
            return;
        }

        output.Append($"<a href=\"{WebUtility.HtmlEncode(href)}\" target=\"_blank\" rel=\"noopener noreferrer\">");
        stack.Push(new OpenTag("url", "</a>"));
    }

    private static void CloseTag(StringBuilder output, Stack<OpenTag> stack, string name)
    {
        if (name == "*")
        {
            name = "li";
        }

        // Ignore stray close tags; otherwise close the match and anything opened inside it
        if (!stack.Any(t => t.Name == name))
        {
            return;
        }

        while (true)
        {
            var top = stack.Pop();
            output.Append(top.CloseHtml);
            if (top.Name == name)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Handles tags whose body is data rather than markup. Consumes up to the matching close tag
    /// and returns the position after it.
    /// </summary>
    private static int AppendRaw(StringBuilder output, string text, int position, string name, bool encodeOnly, out string body, string? value = null)
    {
        var closeTag = $"[/{name}]";
        var closeIndex = text.IndexOf(closeTag, position, StringComparison.OrdinalIgnoreCase);
        var bodyEnd = closeIndex < 0 ? text.Length : closeIndex;
        body = text[position..bodyEnd].Trim();

        output.Append(encodeOnly ? WebUtility.HtmlEncode(text[position..bodyEnd]) : RenderBodyTag(name, body, value));

        return closeIndex < 0 ? text.Length : closeIndex + closeTag.Length;
    }

    private static string RenderBodyTag(string name, string body, string? value)
    {
        switch (name)
        {
            case "img":
                if (body.StartsWith(_steamClanImagePlaceholder, StringComparison.OrdinalIgnoreCase))
                {
                    body = _steamClanImageBase + body[_steamClanImagePlaceholder.Length..].TrimStart('/');
                }

                var src = SanitizeUrl(body);
                return src == null ? string.Empty : $"<img src=\"{WebUtility.HtmlEncode(src)}\" loading=\"lazy\" alt=\"\" />";

            case "previewyoutube":
                var id = value?.Split(';')[0] ?? string.Empty;
                return YouTubeIdRegex().IsMatch(id)
                    ? $"<iframe src=\"https://www.youtube.com/embed/{id}\" allowfullscreen loading=\"lazy\"></iframe>"
                    : string.Empty;

            default:
                // [url]link[/url]
                var href = SanitizeUrl(body);
                var encoded = WebUtility.HtmlEncode(body);
                return href == null
                    ? encoded
                    : $"<a href=\"{WebUtility.HtmlEncode(href)}\" target=\"_blank\" rel=\"noopener noreferrer\">{encoded}</a>";
        }
    }

    private static void AppendText(StringBuilder output, string text, Stack<OpenTag> stack)
    {
        if (text.Length == 0)
        {
            return;
        }

        var encoded = WebUtility.HtmlEncode(text);

        // Newlines between list items are layout, not content
        if (stack.TryPeek(out var current) && current.Name == "li")
        {
            encoded = encoded.TrimEnd('\n');
        }

        if (stack.TryPeek(out var top) && _noLineBreakContainers.Contains(top.Name))
        {
            encoded = encoded.Trim();
        }
        else if (!stack.Any(t => t.Name == "code"))
        {
            encoded = encoded.Replace("\n", "<br />");
        }

        output.Append(encoded);
    }

    private static string? SanitizeUrl(string url)
    {
        return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri.AbsoluteUri
            : null;
    }
}
