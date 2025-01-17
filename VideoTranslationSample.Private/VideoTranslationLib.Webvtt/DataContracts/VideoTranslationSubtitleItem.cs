// <copyright file="VideoTranslationSubtitleItem.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.VideoTranslation;

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SubtitlesParser.Classes;

public class VideoTranslationSubtitleItem : SubtitleItem
{
    public VideoTranslationSubtitleItem()
        : base()
    {
    }

    public static VideoTranslationSubtitleItem Create(SubtitleItem subtitleItem)
    {
        ArgumentNullException.ThrowIfNull(subtitleItem);
        var instance = new VideoTranslationSubtitleItem()
        {
            StartTime = subtitleItem.StartTime,
            EndTime = subtitleItem.EndTime,
        };

        if (subtitleItem.Lines != null)
        {
            instance.Lines.AddRange(subtitleItem.Lines);
        }

        if (subtitleItem.PlaintextLines != null)
        {
            instance.PlaintextLines.AddRange(subtitleItem.PlaintextLines);
        }

        return instance;
    }

    public bool IsJsonMetadataParagraph()
    {
        return string.Equals(this.Lines?.First()?.Trim(), "{", StringComparison.Ordinal);
    }

    public string MergeLinesToString(string delimeter)
    {
        if (string.IsNullOrEmpty(delimeter))
        {
            throw new ArgumentNullException(nameof(delimeter));
        }

        return string.Join(delimeter, this.Lines);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(TimeSpan.FromMilliseconds(this.StartTime).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));
        sb.Append(" --> ");
        sb.Append(TimeSpan.FromMilliseconds(this.EndTime).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));

        if (this.Lines != null)
        {
            sb.AppendLine();
            sb.Append(string.Join(Environment.NewLine, this.Lines));
        }

        return sb.ToString();
    }
}
