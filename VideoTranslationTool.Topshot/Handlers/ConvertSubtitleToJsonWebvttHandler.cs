//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationTool.Topshot.Handlers;

using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.CustomVoice.VideoTranslationWebVtt;
using Microsoft.SpeechServices.VideoTranslationSample.PrivatePreview;
using Microsoft.SpeechServices.VideoTranslationTool.Topshot.Util;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal class ConvertSubtitleToJsonWebvttHandler
{
    public static async Task ConvertSubtitleToJsonWebvttAsync(ConvertSubtitleToJsonWebvttOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!File.Exists(options.SourceSubtitleFilePath))
        {
            throw new FileNotFoundException(options.SourceSubtitleFilePath);
        }

        var (success, error, webVttFile, originalFileKind) = VideoTranslationWebVttFile.Load(
            plainTextKind: VideoTranslationWebVttFilePlainTextKind.TargetLocalePlainText,
            filePath: options.SourceSubtitleFilePath);
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

        await webVttFile.SaveAsJsonMetadataWebvttFileAsync(options.TargetWebvttFilePath).ConfigureAwait(false);
    }
}
