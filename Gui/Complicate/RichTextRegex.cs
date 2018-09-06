using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class Segment
{
    public struct Value
    {
        public bool bold;
        public bool italic;
        public bool underline;
        public int size;
        public UnityEngine.Color32 color;
    }
    public string text;
    public Value value;
}

interface IRichTextRegex
{
    MatchCollection Input(string text);
    Segment Parse(Match match, Segment.Value old);
}
class ColorRegex : IRichTextRegex
{
    public readonly Regex regex = new Regex(@"<color=#[0-9a-fA-F]{8}>(.*?)(</color>)", RegexOptions.Singleline);// 颜色正则
    public MatchCollection Input(string text)
    {
        return regex.Matches(text);
    }
    public Segment Parse(Match match, Segment.Value old)
    {
        var color = match.Value.Substring(8, 8);
        var text = match.Value.Substring(17, match.Value.Length - 25);
        var now = old;
        now.color = Util.UnityHelper.Color32FromString(color);
        return new Segment()
        {
            text = text,
            value = now,
        };
    }
    public static StringBuilder Encode(StringBuilder sb, UnityEngine.Color32 color)
    {
        return sb.Insert(0, "<color=#" + Util.UnityHelper.Color32ToString(color) + ">").Append("</color>");
    }
}
class BoldRegex : IRichTextRegex
{
    public static readonly Regex regex = new Regex(@"<b>(.*?)(</b>)", RegexOptions.Singleline);// 粗体正则
    public MatchCollection Input(string text)
    {
        return regex.Matches(text);
    }
    public Segment Parse(Match match, Segment.Value old)
    {
        var now = old;
        now.bold = true;
        return new Segment()
        {
            text = match.Value.Substring(3, match.Value.Length - 7),
            value = now,
        };
    }
    public static StringBuilder Encode(StringBuilder sb)
    {
        return sb.Insert(0, "<b>").Append("</b>");
    }
}
class ItalicRegex : IRichTextRegex
{
    public static readonly Regex regex = new Regex(@"<i>(.*?)(</i>)", RegexOptions.Singleline);// 斜体正则
    public MatchCollection Input(string text)
    {
        return regex.Matches(text);
    }
    public Segment Parse(Match match, Segment.Value old)
    {
        var now = old;
        now.italic = true;
        return new Segment()
        {
            text = match.Value.Substring(3, match.Value.Length - 7),
            value = now,
        };
    }
    public static StringBuilder Encode(StringBuilder sb)
    {
        return sb.Insert(0, "<i>").Append("</i>");
    }
}
class UnderlineRegex : IRichTextRegex
{
    public static readonly Regex regex = new Regex(@"<u>(.*?)(</u>)", RegexOptions.Singleline);// 斜体正则
    public MatchCollection Input(string text)
    {
        return regex.Matches(text);
    }
    public Segment Parse(Match match, Segment.Value old)
    {
        var now = old;
        now.underline = true;
        return new Segment()
        {
            text = match.Value.Substring(3, match.Value.Length - 7),
            value = now,
        };
    }
    public static StringBuilder Encode(StringBuilder sb)
    {
        return sb.Insert(0, "<u>").Append("</u>");
    }
}
class FontSizeRegex : IRichTextRegex
{
    public static readonly Regex regex = new Regex(@"<size=[0-9]*>(.*?)(</size>)", RegexOptions.Singleline);// 大小正则
    public MatchCollection Input(string text)
    {
        return regex.Matches(text);
    }
    public Segment Parse(Match match, Segment.Value old)
    {
        var now = old;
        var g = match.Groups[1];
        now.size = int.Parse(match.Value.Substring(6, g.Index - match.Index - 7));
        return new Segment()
        {
            text = g.Value,
            value = now,
        };
    }
    public static StringBuilder Encode(StringBuilder sb, int fontSize)
    {
        return sb.Insert(0, "<size=" + fontSize + ">").Append("</size>");
    }
}

class RichTextParser
{
    IList<IRichTextRegex> regexer;
    public RichTextParser()
    {
        regexer = new List<IRichTextRegex>();
        regexer.Add(new ColorRegex());
        regexer.Add(new BoldRegex());
        regexer.Add(new ItalicRegex());
        regexer.Add(new UnderlineRegex());
        regexer.Add(new FontSizeRegex());
    }
    public RichTextParser(params IRichTextRegex[] r)
    {
        regexer = r;
    }
    public RichTextParser(IList<IRichTextRegex> r)
    {
        regexer = r;
    }
    public List<Segment> Parser(string text, Segment.Value sv)
    {
        List<Segment> result = new List<Segment>();
        Parse(new Segment() { text = text, value = sv }, result);
        return result;
    }
    void Parse(Segment seg, List<Segment> finalResult)
    {
        List<Segment> result = new List<Segment>();
        var r = false;
        for (int j = 0; j < regexer.Count; ++j)
        {
            r = Parse(seg, regexer[j], result);
            if (r)
            {
                break;
            }
        }
        if (!r)
        {
            finalResult.Add(seg);
            return;
        }
        for (int index = 0; index < result.Count; ++index)
        {
            Parse(result[index], finalResult);
        }
    }
    bool Parse(Segment segment, IRichTextRegex regex, IList<Segment> result)
    {
        var matches = regex.Input(segment.text);
        if (matches.Count > 0)
        {
            var start = matches[0];
            if (start.Index > 0)
            {
                result.Add(new Segment()
                {
                    text = segment.text.Substring(0, start.Index),
                    value = segment.value,
                });
            }

            for (int j = 0; j < matches.Count; ++j)
            {
                var match = matches[j];
                result.Add(regex.Parse(match, segment.value));
            }

            var last = matches[matches.Count - 1];
            if (last.Index + last.Length < segment.text.Length)
            {
                result.Add(new Segment()
                {
                    text = segment.text.Substring(last.Index + last.Length, segment.text.Length - (last.Index + last.Length)),
                    value = segment.value,
                });
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}