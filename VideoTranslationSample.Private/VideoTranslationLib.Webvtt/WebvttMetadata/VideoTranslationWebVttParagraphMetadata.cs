// <copyright file="VideoTranslationWebVttParagraphMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib.Enums;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.VideoTranslationLib.PrivateBase;
using System;

public class VideoTranslationWebVttParagraphMetadata : VideoTranslationWebVttMetadataBase
{
    public VideoTranslationWebVttGlobalMetadata GlobalMetadata { get; set; }

    public Guid? Id { get; set; }

    // STCI need support voice selection by gender, if gender conflict with voice name, then customer can specify using gender as high priority.
    public Gender? Gender { get; set; }

    // Speaker ID, don't support specify voice type and voice name in paragraph to keep consistence with portal.
    public string SpeakerId { get; set; }

    public bool? KeepOriginal { get; set; }

    public VideoTranslationWebVttSsmlMetadata SsmlProperties { get; set; }

    public VideoTranslationWebVttTtsMetadata TtsProperties { get; set; }

    public VideoTranslationWebVttTranslationProperties TranslationProperties { get; set; }

    public string SourceLocaleText { get; set; }

    public string VideoSubtitleText { get; set; }

    public string TranslatedText { get; set; }

    public VideoTranslationMergeParagraphAudioAlignKind? AlignKind { get; set; }

    public bool? DisableTextTokenizer { get; set; }

    public static VideoTranslationWebVttParagraphMetadata CreateFromSubtitlePlainText(
        VideoTranslationWebVttFilePlainTextKind plainTextKind,
        string subtitlePlainText)
    {
        var instance = new VideoTranslationWebVttParagraphMetadata()
        {
            Id = Guid.NewGuid(),
        };

        if (string.Equals(
            subtitlePlainText,
            VideoTranslationConst.KeepOriginalSubtitlePlaceHolder,
            StringComparison.OrdinalIgnoreCase))
        {
            instance.KeepOriginal = true;
        }
        else
        {
            switch (plainTextKind)
            {
                case VideoTranslationWebVttFilePlainTextKind.SourceLocalePlainText:
                    {
                        instance.SourceLocaleText = subtitlePlainText;
                        break;
                    }

                case VideoTranslationWebVttFilePlainTextKind.TargetLocalePlainText:
                    {
                        instance.TranslatedText = subtitlePlainText;
                        break;
                    }

                default:
                    throw new NotSupportedException(plainTextKind.AsString());
            }
        }

        return instance;
    }

    public override bool HasValue()
    {
        return base.HasValue() ||
            !string.IsNullOrEmpty(this.SpeakerId) ||
            (this.SsmlProperties?.HasValue() ?? false) ||
            !string.IsNullOrEmpty(this.TranslatedText) ||
            !string.IsNullOrEmpty(this.VideoSubtitleText) ||
            (this.GlobalMetadata != null && this.GlobalMetadata.HasValue());
    }

    public VideoTranslationWebVttParagraphMetadata Clone()
    {
        var other = new VideoTranslationWebVttParagraphMetadata();

        other.LoadForBaseClass(this);

        other.Id = this.Id;
        other.Gender = this.Gender;
        other.SpeakerId = this.SpeakerId;
        other.KeepOriginal = this.KeepOriginal;
        other.SsmlProperties = this.SsmlProperties?.CloneSsmlMetadata();
        other.SourceLocaleText = this.SourceLocaleText;
        other.VideoSubtitleText = this.VideoSubtitleText;
        other.TranslatedText = this.TranslatedText;
        other.AlignKind = this.AlignKind;

        if (this.GlobalMetadata == null)
        {
            other.GlobalMetadata = null;
        }
        else
        {
            other.GlobalMetadata = this.GlobalMetadata.Clone();
        }

        return other;
    }
}
