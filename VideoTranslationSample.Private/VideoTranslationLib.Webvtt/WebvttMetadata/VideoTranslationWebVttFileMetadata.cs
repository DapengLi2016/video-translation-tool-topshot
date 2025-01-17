// <copyright file="VideoTranslationWebVttFileMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// AudioAlignKind is specified when create translation.
public class VideoTranslationWebVttFileMetadata : VideoTranslationWebVttMetadataBase
{
    public VideoTranslationWebVttFileMetadata()
    {
        Speakers = new Dictionary<string, VideoTranslationWebVttSpeakerMetadata>();
    }

    public string Schema { get; set; }

    public CultureInfo Locale { get; set; }

    public string VoiceStyle { get; set; }

    // If speaker not registered, then it is zero shot.
    public Dictionary<string, VideoTranslationWebVttSpeakerMetadata> Speakers { get; private set; }

    public VideoTranslationWebVttFileMetadata Clone()
    {
        var instance = new VideoTranslationWebVttFileMetadata()
        {
            Schema = Schema,
            Locale = Locale ?? CultureInfo.CreateSpecificCulture(Locale.Name),
            VoiceStyle = VoiceStyle,
            Comment = Comment,
        };

        instance.Speakers = Speakers?.ToDictionary(x => x.Key, x => x.Value.Clone());
        return instance;
    }
}
