// <copyright file="VideoTranslationWebVttGlobalMetadata.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.SpeechServices.Common;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib.Enums;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.CustomVoice.Util;
using Microsoft.SpeechServices.VideoTranslationLib.PrivateBase;

public class VideoTranslationWebVttGlobalMetadata : VideoTranslationWebVttMetadataBase
{
    // Due to mutiple thread need register speaker during voice selection durin synthesis, so need lock.
    private object speakerWriteLock = new object();

    public VideoTranslationWebVttGlobalMetadata()
    {
        Speakers = new SortedDictionary<string, VideoTranslationWebVttSpeakerMetadata>();
    }

    public CultureInfo Locale { get; set; }

    public VideoTranslationWebVttDefaultSsmlMetadata DefaultSsmlProperties { get; set; }

    public VideoTranslationMergeParagraphAudioAlignKind? DefaultAlignKind { get; set; }

    // By default voice has higher priority then gender.
    // While STCI want to use gender as high priority to override voice, so specify this explicitly.
    public bool? ChangeTtsVoiceNameIfConflictWithGender { get; set; }

    public SortedDictionary<string, VideoTranslationWebVttSpeakerMetadata> Speakers { get; private set; }

    public int SpeakerCount()
    {
        return Speakers?.Count(x => !string.IsNullOrWhiteSpace(x.Key)) ?? 0;
    }

    public string FirstOrDefaultPersonalVoiceSpeaker()
    {
        if (Speakers == null)
        {
            return null;
        }

        // Not use LINQ .FirstOrDefault due to KeyVaulePair is struct.
        foreach (var pair in Speakers.Where(x => x.Value?.DefaultSsmlProperties?.VoiceKind == VideoTranslationVoiceKind.PersonalVoice))
        {
            return pair.Key;
        }

        return null;
    }

    public void RegisterSpeakerIfNotExist(string speakerName)
    {
        if (string.IsNullOrEmpty(speakerName))
        {
            throw new ArgumentNullException(nameof(speakerName));
        }

        if (!Speakers.ContainsKey(speakerName))
        {
            Speakers[speakerName] = new VideoTranslationWebVttSpeakerMetadata();
        }
    }

    public override bool HasValue()
    {
        return base.HasValue() ||
            !string.IsNullOrEmpty(Locale?.Name) ||
            (DefaultSsmlProperties?.HasValue() ?? false) ||
            (Speakers?.Keys?.Any() ?? false);
    }

    public (string speakerId,
        VideoTranslationWebVttSpeakerMetadata speakerMetadata)
    RegisterSpakerOfVoiceKindIfNotExist(VideoTranslationVoiceKind voiceKind, string speakerId)
    {
        lock (speakerWriteLock)
        {
            var standardizedSpeakerId = speakerId;
            if (string.IsNullOrEmpty(speakerId))
            {
                standardizedSpeakerId = voiceKind == VideoTranslationVoiceKind.PersonalVoice ?
                    VideoTranslationConst.DefaultPersonalVoiceSpeakerName :
                    VideoTranslationConst.DefaultOverDubbingSpeakerName;
            }

            var (found, existingSpeakerId, speakerMetadata) = FindSpakerOfVoiceKind(voiceKind, standardizedSpeakerId);
            if (found)
            {
                return (standardizedSpeakerId, speakerMetadata);
            }

            if (Speakers == null)
            {
                Speakers = new SortedDictionary<string, VideoTranslationWebVttSpeakerMetadata>();
            }

            // Assign default ssml properties, due to the global default value may be come from API request parameter.
            var speakerDefaultSsmlProperties = new VideoTranslationWebVttSpeakerDefaultSsmlMetadata()
            {
                VoiceKind = voiceKind,
            };

            if (DefaultSsmlProperties != null)
            {
                speakerDefaultSsmlProperties.LoadForDefaultSsmlMetadata(DefaultSsmlProperties);
            }

            // If the same speakerId of the different voice kind before, then in function this.FindSpakerOfVoiceKind will not find it.
            // Then here will overwrite the speaker ID with the new voice kind.
            Speakers[standardizedSpeakerId] = new VideoTranslationWebVttSpeakerMetadata()
            {
                DefaultSsmlProperties = speakerDefaultSsmlProperties,
            };

            return (standardizedSpeakerId, Speakers[standardizedSpeakerId]);
        }
    }

    public (
        bool found,
        string speakerId,
        VideoTranslationWebVttSpeakerMetadata speakerMetadata)
    FindSpakerOfVoiceKind(
        VideoTranslationVoiceKind voiceKind,
        string speakerId)
    {
        if (string.IsNullOrWhiteSpace(speakerId))
        {
            throw new ArgumentNullException(nameof(speakerId));
        }

        if (voiceKind == VideoTranslationVoiceKind.None)
        {
            throw new ArgumentException($"{nameof(voiceKind)} should not be {VideoTranslationVoiceKind.None.AsString()}");
        }

        var func = (KeyValuePair<string, VideoTranslationWebVttSpeakerMetadata> x) =>
            string.Equals(speakerId, x.Key, StringComparison.OrdinalIgnoreCase) &&
            x.Value?.DefaultSsmlProperties?.VoiceKind == voiceKind;

        if (!(Speakers?.Any(x => func(x)) ?? false))
        {
            return (false, null, null);
        }

        var speakerPair = Speakers.First(x => func(x));
        return (true, speakerPair.Key, speakerPair.Value);
    }

    public (
        bool found,
        string speakerId,
        VideoTranslationWebVttSpeakerMetadata speakerMetadata)
    FindSpakerOfVoiceKind(
        string voiceName,
        VideoTranslationVoiceKind voiceKind)
    {
        if (Speakers == null ||
            !Speakers.Any(x =>
                x.Value.DefaultSsmlProperties?.VoiceKind == voiceKind &&
                string.Equals(x.Value.DefaultSsmlProperties?.VoiceName, voiceName, StringComparison.Ordinal)))
        {
            return (false, null, null);
        }

        var speakerPair = Speakers.First(x =>
            x.Value.DefaultSsmlProperties?.VoiceKind == voiceKind &&
            string.Equals(x.Value.DefaultSsmlProperties?.VoiceName, voiceName, StringComparison.Ordinal));
        return (true, speakerPair.Key, speakerPair.Value);
    }

    public VideoTranslationWebVttGlobalMetadata Clone()
    {
        var other = new VideoTranslationWebVttGlobalMetadata();

        other.LoadForBaseClass(this);

        other.Locale = Locale;
        other.DefaultSsmlProperties = DefaultSsmlProperties?.CloneDefaultSsmlMetadata();
        other.DefaultAlignKind = DefaultAlignKind;
        other.ChangeTtsVoiceNameIfConflictWithGender = ChangeTtsVoiceNameIfConflictWithGender;

        if (Speakers != null)
        {
            foreach (var speaker in Speakers)
            {
                other.Speakers[speaker.Key] = speaker.Value?.Clone();
            }
        }

        return other;
    }

    private string BuildNewSpakerIdOfVoiceKind(string voiceName)
    {
        var name = VideoTranslationSpeakerHelper.BuildPlatformVoiceSpakerId(voiceName);
        if (Speakers.ContainsKey(name))
        {
            name = $"{name}_{Guid.NewGuid()}";
        }

        return name;
    }
}
