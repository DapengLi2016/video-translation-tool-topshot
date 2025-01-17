// <copyright file="VideoTranslationWebVttSpeakerMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

using System;
using System.Collections.Generic;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

public class VideoTranslationWebVttSpeakerMetadata : VideoTranslationWebVttMetadataBase
{
    public Guid? TtsCustomVoiceDeploymentId { get; set; }

    // User provided consent ID.
    public string ConsentId { get; set; }

    public VideoTranslationWebVttSpeakerDefaultSsmlMetadata DefaultSsmlProperties { get; set; }

    public override bool HasValue()
    {
        return base.HasValue() ||
            (this.TtsCustomVoiceDeploymentId != null && this.TtsCustomVoiceDeploymentId != Guid.Empty) ||
            (this.DefaultSsmlProperties?.HasValue() ?? false);
    }

    public VideoTranslationWebVttSpeakerMetadata Clone()
    {
        var other = new VideoTranslationWebVttSpeakerMetadata();
        other.LoadForBaseClass(this);

        other.TtsCustomVoiceDeploymentId = this.TtsCustomVoiceDeploymentId;
        other.DefaultSsmlProperties = this.DefaultSsmlProperties?.CloneSpeakerDefaultSsmlMetadata();
        return other;
    }
}
