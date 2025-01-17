// <copyright file="VideoTranslationWebVttSsmlMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

using System;

// Voice name is specified as speaker in global setting.
// Not specify voice name in segment metadata.
public class VideoTranslationWebVttSsmlMetadata : VideoTranslationWebVttDefaultSsmlMetadata
{
    public string XmlContentInVoiceTag { get; set; }

    public override bool HasValue()
    {
        return base.HasValue() ||
            !string.IsNullOrEmpty(this.XmlContentInVoiceTag);
    }

    public void LoadForSsmlMetadata(VideoTranslationWebVttSsmlMetadata other)
    {
        ArgumentNullException.ThrowIfNull(other);

        this.LoadForDefaultSsmlMetadata(other);

        this.XmlContentInVoiceTag = other.XmlContentInVoiceTag;
    }

    public VideoTranslationWebVttSsmlMetadata CloneSsmlMetadata()
    {
        var newInstance = new VideoTranslationWebVttSsmlMetadata();
        newInstance.LoadForSsmlMetadata(this);
        return newInstance;
    }
}
