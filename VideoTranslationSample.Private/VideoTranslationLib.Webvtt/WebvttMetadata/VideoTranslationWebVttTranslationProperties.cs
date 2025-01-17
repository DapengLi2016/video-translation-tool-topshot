// <copyright file="VideoTranslationWebVttTranslationProperties.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

using Microsoft.SpeechServices.Common.Client;

public class VideoTranslationWebVttTranslationProperties
{
    public int? EstimatedTranslatedCharDelta { get; set; }

    public string MatchedConsentId { get; set; }

    public VideoTranslationVoiceKind? TtsSynthesisVoiceKind { get; set; }
}
