// <copyright file="VideoTranslationWebVttDefaultSsmlMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

using System;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

public class VideoTranslationWebVttDefaultSsmlMetadata : VideoTranslationWebVttMetadataBase
{
    public VideoTranslationVoiceKind? VoiceKind { get; set; }

    public bool? EnableProsodyTransfer { get; set; }

    public string StyleName { get; set; }

    public double? StyleDegree { get; set; }

    public double? Rate { get; set; }

    public override bool HasValue()
    {
        return base.HasValue() ||
            (this.VoiceKind ?? VideoTranslationVoiceKind.None) != VideoTranslationVoiceKind.None ||
            (this.EnableProsodyTransfer ?? false) ||
            !string.IsNullOrEmpty(this.StyleName) ||
            this.StyleDegree != null ||
            this.Rate != null;
    }

    public void LoadForDefaultSsmlMetadata(VideoTranslationWebVttDefaultSsmlMetadata other)
    {
        ArgumentNullException.ThrowIfNull(other);

        this.LoadForBaseClass(other);

        this.EnableProsodyTransfer = other.EnableProsodyTransfer;
        this.StyleName = other.StyleName;
        this.StyleDegree = other.StyleDegree;
        this.Rate = other.Rate;
    }

    public VideoTranslationWebVttDefaultSsmlMetadata CloneDefaultSsmlMetadata()
    {
        var newInstance = new VideoTranslationWebVttDefaultSsmlMetadata();
        newInstance.LoadForDefaultSsmlMetadata(this);
        return newInstance;
    }
}
