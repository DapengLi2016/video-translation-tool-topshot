//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationTool.Topshot.Handlers;

using Microsoft.SpeechServices.Common;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.CustomVoice.VideoTranslationWebVtt;
using Microsoft.SpeechServices.VideoTranslationTool.Topshot.Util;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal class ConvertSubtitleToJsonWebvttHandler
{
    public static async Task ConvertSubtitleDirToJsonWebvttAsync(string sourceSubtitleDirPath, string targetWebvttDirPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSubtitleDirPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetWebvttDirPath);

        if (!Directory.Exists(sourceSubtitleDirPath))
        {
            throw new DirectoryNotFoundException(sourceSubtitleDirPath);
        }

        var dirInfo = new FileInfo(sourceSubtitleDirPath);
        foreach (var filePath in Directory.GetFiles(dirInfo.FullName, $"*.{PrivateFileNameExtensions.WebVttFile}", SearchOption.AllDirectories))
        {
            // Due to SourceSubtitleDirPath maybe relative path, so get absulate path for calculating relative path.
            var fileInfo = new FileInfo(filePath);

            var relativePath = fileInfo.FullName.Substring(dirInfo.FullName.Length + 1);
            var targetFilePath = Path.Combine(targetWebvttDirPath, relativePath);
            await ConvertSubtitleFileToJsonWebvttAsync(fileInfo.FullName, targetFilePath).ConfigureAwait(false);
        }
    }

    public static async Task ConvertSubtitleFileToJsonWebvttAsync(string sourceSubtitleFilePath, string targetWebvttFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceSubtitleFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetWebvttFilePath);

        if (!File.Exists(sourceSubtitleFilePath))
        {
            throw new FileNotFoundException(sourceSubtitleFilePath);
        }

        var (success, error, webVttFile, originalFileKind) = VideoTranslationWebVttFile.Load(
            plainTextKind: VideoTranslationWebVttFilePlainTextKind.TargetLocalePlainText,
            filePath: sourceSubtitleFilePath);
        if (!success)
        {
            throw new InvalidDataException(error);
        }

        if (webVttFile.Paragraphs != null)
        {
            foreach (var paragraph in webVttFile.Paragraphs
                .Where(x => !string.IsNullOrEmpty(x.ParagraphMetadata?.TranslatedText)))
            {
                var (speakerName, subtitle) = TopshotHelper.IsMatchSubtitleTimeFormat(paragraph.ParagraphMetadata.TranslatedText);
                if (!string.IsNullOrEmpty(speakerName))
                {
                    paragraph.ParagraphMetadata.SpeakerId = speakerName;
                    webVttFile.RegisterSpakerIdIfNotExist(
                        voiceKind: VideoTranslationVoiceKind.PlatformVoice,
                        speakerId: speakerName);
                }

                paragraph.ParagraphMetadata.TranslatedText = subtitle;
            }
        }

        await webVttFile.SaveAsJsonMetadataWebvttFileAsync(targetWebvttFilePath).ConfigureAwait(false);
    }
}
