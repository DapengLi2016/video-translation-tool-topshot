// <copyright file="VideoTranslationWebVttParagraph.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.VideoTranslationWebVtt;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CustomVoice.VideoTranslation;
using Microsoft.SpeechServices.CustomVoice.WebVtt;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;
using Newtonsoft.Json;

public class VideoTranslationWebVttParagraph
{
    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public VideoTranslationWebVttParagraphMetadata ParagraphMetadata { get; set; }

    [JsonIgnore]
    public TimeSpan Duration
    {
        get
        {
            if (this.EndTime <= this.StartTime)
            {
                return TimeSpan.Zero;
            }

            return this.EndTime - this.StartTime;
        }
    }

    public static (bool success, string error, VideoTranslationWebVttParagraph paragraph, VideoTranslationWebvttFileKind? originalFileKind) Create(
        VideoTranslationWebVttFilePlainTextKind plainTextKind,
        VideoTranslationSubtitleItem subtitleItem)
    {
        ArgumentNullException.ThrowIfNull(subtitleItem);
        var instance = new VideoTranslationWebVttParagraph()
        {
            StartTime = TimeSpan.FromMilliseconds(subtitleItem.StartTime),
            EndTime = TimeSpan.FromMilliseconds(subtitleItem.EndTime),
        };

        if (subtitleItem.Lines == null)
        {
            return (true, null, instance, null);
        }

        VideoTranslationWebvttFileKind? originalFileKind = null;
        if (subtitleItem.IsJsonMetadataParagraph())
        {
            var (success, error, paragraphMetadata) = JsonHelper.TryParse<VideoTranslationWebVttParagraphMetadata>(
                subtitleItem.MergeLinesToString(Environment.NewLine));
            if (success)
            {
                originalFileKind = VideoTranslationWebvttFileKind.MetadataJson;
                instance.ParagraphMetadata = paragraphMetadata;
            }
            else
            {
                // For invalid json format.
                return (false, $"Invalid json format for paragraph with error [{error}] for paragraph with content [{subtitleItem.MergeLinesToString(" ")}]", null, null);
            }
        }
        else
        {
            // For plain text, use it as plain text.
            originalFileKind = plainTextKind == VideoTranslationWebVttFilePlainTextKind.SourceLocalePlainText ?
                VideoTranslationWebvttFileKind.SourceLocaleSubtitle : VideoTranslationWebvttFileKind.TargetLocaleSubtitle;
            instance.ParagraphMetadata = VideoTranslationWebVttParagraphMetadata.CreateFromSubtitlePlainText(
                plainTextKind: plainTextKind,
                subtitlePlainText: subtitleItem.MergeLinesToString(Environment.NewLine));
        }

        return (true, null, instance, originalFileKind);
    }

    public VideoTranslationWebVttParagraph Clone()
    {
        return new VideoTranslationWebVttParagraph()
        {
            StartTime = this.StartTime,
            EndTime = this.EndTime,
            ParagraphMetadata = this.ParagraphMetadata?.Clone(),
        };
    }

    public VideoTranslationSubtitleItem ToJsonMetadataSubTitleItem()
    {
        var item = new VideoTranslationSubtitleItem()
        {
            StartTime = (int)this.StartTime.TotalMilliseconds,
            EndTime = (int)this.EndTime.TotalMilliseconds,
        };

        var lines = new List<string>();
        if (this.ParagraphMetadata != null)
        {
            var json = JsonConvert.SerializeObject(this.ParagraphMetadata, Formatting.Indented, CommonPublicConst.Json.WriterSettings);
            lines.AddRange(json.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
        }

        item.Lines.AddRange(lines);
        return item;
    }
}
