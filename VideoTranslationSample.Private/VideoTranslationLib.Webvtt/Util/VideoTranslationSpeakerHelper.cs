// <copyright file="VideoTranslationSpeakerHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpeechServices.Common;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib.Util;

public static class VideoTranslationSpeakerHelper
{
    public static string BuildPlatformVoiceSpakerId(string voiceName)
    {
        if (string.IsNullOrEmpty(voiceName))
        {
            throw new ArgumentNullException(nameof(voiceName));
        }

        return $"{VideoTranslationVoiceKind.PlatformVoice.AsString()}_{voiceName}";
    }
}
