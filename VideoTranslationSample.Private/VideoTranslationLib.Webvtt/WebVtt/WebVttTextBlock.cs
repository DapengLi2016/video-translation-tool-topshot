// <copyright file="WebVttTextBlock.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.WebVtt;

using Microsoft.SpeechServices.VideoTranslationLib.PublicBase.Enum;
using Microsoft.SpeechServices.VideoTranslationLib.Webvtt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class WebVttTextBlock
{
    public WebVttTextBlock()
    {
        this.Lines = new List<WebVttTextLine>();
    }

    public List<WebVttTextLine> Lines { get; private set; }

    public WebVttTextBlock Clone()
    {
        return new WebVttTextBlock()
        {
            Lines = this.Lines?.Select(x => x.Clone())?.ToList(),
        };
    }

    public async Task WriteAsync(StreamWriter stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        foreach (var line in this.Lines)
        {
            await stream.WriteLineAsync(line.Text).ConfigureAwait(false);
        }
    }

    public WebVttTextBlockKind DetectTextBlockVideoTranslationKind()
    {
        if (this.Lines.Count == 1 && string.Equals(this.Lines.First().Text, WebvttConstant.FileHeaderText, StringComparison.Ordinal))
        {
            return WebVttTextBlockKind.Header;
        }
        else if (this.Lines.Count > 1)
        {
            if (WebVttSubtitle.IsMatchSubtitleTimeFormat(this.Lines[0].Text).success)
            {
                return WebVttTextBlockKind.Subtitle;
            }
            else if (int.TryParse(this.Lines[0].Text, out var lineNumber)
                && lineNumber > 0 &&
                WebVttSubtitle.IsMatchSubtitleTimeFormat(this.Lines[1].Text).success)
            {
                return WebVttTextBlockKind.SubtitleWithParagraphOrder;
            }
        }

        return WebVttTextBlockKind.Unknown;
    }
}
