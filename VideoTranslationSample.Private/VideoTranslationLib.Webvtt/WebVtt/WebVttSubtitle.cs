// <copyright file="WebVttSubtitle.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.WebVtt;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt;
using Newtonsoft.Json;

public class WebVttSubtitle
{
    public WebVttSubtitle()
    {
        this.Lines = new ConcurrentDictionary<int, string>();
    }

    public WebVttSubtitle(string line)
        : this()
    {
        this.Lines[1] = line;
    }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [JsonIgnore]
    public ConcurrentDictionary<int, string> Lines { get; private set; }

    [JsonIgnore]
    public IEnumerable<string> LinesText => this.SortedLines?.Select(x => x.Value);

    [DataMember]
    public ImmutableSortedDictionary<int, string> SortedLines =>
        this.Lines?.ToImmutableSortedDictionary(
            x => x.Key,
            x => x.Value);

    public static (bool success, TimeSpan startTime, TimeSpan endTime) IsMatchSubtitleTimeFormat(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return (false, TimeSpan.Zero, TimeSpan.Zero);
        }

        var match = WebvttConstant.SubtitleTimelineMatch.Match(line);
        if (!match.Success || !match.Groups.ContainsKey("startTime") || !match.Groups.ContainsKey("endTime"))
        {
            return (false, TimeSpan.Zero, TimeSpan.Zero);
        }

        return (
            true,
            WebvttHelper.ParseTimeSpan(match.Groups["startTime"].Value),
            WebvttHelper.ParseTimeSpan(match.Groups["endTime"].Value));
    }

    public WebVttSubtitle Clone()
    {
        var instance = new WebVttSubtitle()
        {
            StartTime = this.StartTime,
            EndTime = this.EndTime,
        };

        if (this.Lines != null)
        {
            foreach (var (key, value) in this.Lines)
            {
                instance.Lines[key] = value;
            }
        }

        return instance;
    }

    public TimeSpan Duration()
    {
        if (this.EndTime < this.StartTime)
        {
            return TimeSpan.Zero;
        }

        return this.EndTime - this.StartTime;
    }

    public string ToSynthesisText()
    {
        return string.Join(' ', this.LinesText ?? Enumerable.Empty<string>());
    }

    public string ToWebVttText()
    {
        var sb = new StringBuilder();
        sb.Append(this.StartTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));
        sb.Append(" --> ");
        sb.Append(this.EndTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));

        var text = string.Join(Environment.NewLine, this.LinesText ?? Enumerable.Empty<string>());
        if (!string.IsNullOrEmpty(text))
        {
            sb.AppendLine();
            sb.Append(text);
        }

        return sb.ToString();
    }
}
