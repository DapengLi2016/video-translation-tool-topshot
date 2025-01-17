// <copyright file="VideoTranslationWebVttTtsMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;

using System;
using Newtonsoft.Json;

public class VideoTranslationWebVttTtsMetadata
{
    public TimeSpan? PersonalVoicePromptAudioStart { get; set; }

    public TimeSpan? PersonalVoicePromptAudioEnd { get; set; }

    [JsonIgnore]
    public TimeSpan? PersonalVoicePromptAudioDuration
    {
        get
        {
            if (this.PersonalVoicePromptAudioStart == null ||
                this.PersonalVoicePromptAudioEnd == null)
            {
                return null;
            }

            return this.PersonalVoicePromptAudioEnd.Value - this.PersonalVoicePromptAudioStart.Value;
        }
    }
}
