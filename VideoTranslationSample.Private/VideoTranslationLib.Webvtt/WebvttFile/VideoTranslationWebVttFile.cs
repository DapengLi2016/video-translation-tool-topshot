// <copyright file="VideoTranslationWebVttFile.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.VideoTranslationWebVtt;

using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.TtsUtil;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.WebvttMetadata;
using Microsoft.SpeechServices.CustomVoice.VideoTranslation;
using Microsoft.SpeechServices.CustomVoice.WebVtt;
using Microsoft.SpeechServices.VideoTranslationLib.PublicBase.Enum;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt.WebvttMetadata;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Not use Nuget library, becuase video translation need keep NOTE part order and attach it to the above paragraph subtitle.
/// Other library can't handle this well, so implement it here.
/// </summary>
public class VideoTranslationWebVttFile
{
    public VideoTranslationWebVttFile()
    {
        this.Paragraphs = new List<VideoTranslationWebVttParagraph>();
    }

    public List<VideoTranslationWebVttParagraph> Paragraphs { get; private set; }

    public static (bool success, string error, VideoTranslationWebVttFile file, VideoTranslationWebvttFileKind? originalFileKind) Load(
        VideoTranslationWebVttFilePlainTextKind plainTextKind,
        string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        var parser = new SubtitlesParser.Classes.Parsers.VttParser();
        using (var fileReadStream = File.OpenRead(filePath))
        {
            try
            {
                var items = parser.ParseStream(fileReadStream, Encoding.UTF8);
                if (items == null)
                {
                    return (false, $"Failed to parse webvtt file.", null, null);
                }

                return Create(
                    plainTextKind: plainTextKind,
                    subtitleItems: items.Select(x => VideoTranslationSubtitleItem.Create(x)));
            }
            catch (FormatException e)
            {
                return (false, $"Parse webvtt failed with {typeof(FormatException).Name} error: {e.Message}", null, null);
            }
        }
    }

    public static (bool success, string error, VideoTranslationWebVttFile file, VideoTranslationWebvttFileKind? originalFileKind) Create(
        VideoTranslationWebVttFilePlainTextKind plainTextKind,
        IEnumerable<VideoTranslationSubtitleItem> subtitleItems)
    {
        ArgumentNullException.ThrowIfNull(subtitleItems);
        var file = new VideoTranslationWebVttFile();
        VideoTranslationWebvttFileKind? originalFileKind = null;
        foreach (var item in subtitleItems)
        {
            var (success, error, paragraph, paragraphOriginalFileKind) = VideoTranslationWebVttParagraph.Create(
                plainTextKind: plainTextKind,
                subtitleItem: item);
            if (!success)
            {
                return (false, error, null, null);
            }

            if (originalFileKind == null && paragraphOriginalFileKind != null)
            {
                originalFileKind = paragraphOriginalFileKind;
            }

            file.Paragraphs.Add(paragraph);
        }

        return (true, null, file, originalFileKind);
    }

    public VideoTranslationWebVttFile Clone()
    {
        var instance = new VideoTranslationWebVttFile();
        if (this.Paragraphs != null)
        {
            foreach (var paragraph in this.Paragraphs.Select(x => x.Clone()))
            {
                instance.Paragraphs.Add(paragraph);
            }
        }

        return instance;
    }

    public (bool success,
        string error,
        string speakerId,
        VideoTranslationWebVttSpeakerMetadata speakerMetadata)
    RegisterSpakerIdIfNotExist(
        VideoTranslationVoiceKind voiceKind,
        string speakerId)
    {
        var (success, error, globalMetadata) = this.CreateGlobalMetadataIfNotExist();
        if (!success)
        {
            return (false, error, null, null);
        }

        var (_, speakerMetadata) = globalMetadata.RegisterSpakerOfVoiceKindIfNotExist(
            voiceKind: voiceKind,
            speakerId: speakerId);
        return (true, null, speakerId, speakerMetadata);
    }

    public (bool success, string error, VideoTranslationWebVttGlobalMetadata globalMetadata) CreateGlobalMetadataIfNotExist()
    {
        if (this.Paragraphs == null || !this.Paragraphs.Any())
        {
            return (false, "Please add at least one paragraph.", null);
        }

        var paragraph = this.Paragraphs.First();
        if (paragraph.ParagraphMetadata == null)
        {
            paragraph.ParagraphMetadata = new VideoTranslationWebVttParagraphMetadata();
        }

        if (paragraph.ParagraphMetadata.GlobalMetadata == null)
        {
            paragraph.ParagraphMetadata.GlobalMetadata = new VideoTranslationWebVttGlobalMetadata();
        }

        return (true, null, paragraph.ParagraphMetadata.GlobalMetadata);
    }

    public async Task SaveAsJsonMetadataWebvttFileAsync(string filepath)
    {
        if (string.IsNullOrEmpty(filepath))
        {
            throw new ArgumentNullException(nameof(filepath));
        }

        CommonHelper.EnsureFolderExistForFile(filepath);

        // Not use File.OpenWrite, due to it use FileMode.OpenOrCreate, which has problem, if file path already exist, but new file is smaller, then it will only overwrite leading parts, left parts from old file will still be there, which cuase the new file is not correct.
        using var fileWriteStream = File.Open(filepath, FileMode.Create, FileAccess.Write);

        await WebvttHelper.WriteStreamAsync(
            fileWriteStream,
            this.Paragraphs.Select(x => x.ToJsonMetadataSubTitleItem())).ConfigureAwait(false);
    }
}
