// <copyright file="VideoTranslationWebVttSpeakerDefaultSsmlMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpeechServices.Common.Client;

public class VideoTranslationWebVttSpeakerDefaultSsmlMetadata : VideoTranslationWebVttDefaultSsmlMetadata
{
    // Not define gender in this metadata, due to voice name has specific gender.
    public string VoiceName { get; set; }

    public bool IsPlatformVoiceProsodyTransfer()
    {
        return (this.VoiceKind == VideoTranslationVoiceKind.PlatformVoice) &&
            !string.IsNullOrWhiteSpace(this.VoiceName) &&
            (this.EnableProsodyTransfer ?? false);
    }

    public override bool HasValue()
    {
        return base.HasValue() ||
            (this.VoiceKind ?? VideoTranslationVoiceKind.None) != VideoTranslationVoiceKind.None ||
            !string.IsNullOrEmpty(this.VoiceName);
    }

    public void LoadForSpeakerDefaultSsmlMetadata(VideoTranslationWebVttSpeakerDefaultSsmlMetadata other)
    {
        ArgumentNullException.ThrowIfNull(other);

        this.LoadForDefaultSsmlMetadata(other);

        this.VoiceKind = other.VoiceKind;
        this.VoiceName = other.VoiceName;
    }

    public VideoTranslationWebVttSpeakerDefaultSsmlMetadata CloneSpeakerDefaultSsmlMetadata()
    {
        var newInstance = new VideoTranslationWebVttSpeakerDefaultSsmlMetadata();

        newInstance.LoadForDefaultSsmlMetadata(this);

        newInstance.VoiceKind = this.VoiceKind;
        newInstance.VoiceName = this.VoiceName;

        return newInstance;
    }
}
