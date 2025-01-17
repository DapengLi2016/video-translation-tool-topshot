// <copyright file="VideoTranslationWebVttMetadataBase.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

public class VideoTranslationWebVttMetadataBase
{
    public string Comment { get; set; }

    public virtual bool HasValue()
    {
        return !string.IsNullOrEmpty(Comment);
    }

    public void LoadForBaseClass(VideoTranslationWebVttMetadataBase other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Comment = other.Comment;
    }
}
