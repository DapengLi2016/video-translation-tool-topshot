// <copyright file="WebVttFile.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.WebVtt;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SpeechServices.CommonLib.TtsUtil;
using Microsoft.SpeechServices.CustomVoice.TtsLib.TtsUtil;

/// <summary>
/// Not use Nuget library, becuase video translation need keep NOTE part order and attach it to the above paragraph subtitle.
/// Other library can't handle this well, so implement it here.
/// </summary>
public class WebVttFile
{
    public WebVttFile()
    {
        this.TextBlocks = new List<WebVttTextBlock>();
    }

    public List<WebVttTextBlock> TextBlocks { get; private set; }

    public void Load(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        this.Load(fs);
    }

    public void Load(Stream stream)
    {
        this.TextBlocks.Clear();
        this.TextBlocks.Add(new WebVttTextBlock());

        var lineNumber = 0;
        foreach (var line in CommonHelper.FileLines(stream, Encoding.UTF8))
        {
            lineNumber++;
            if (string.IsNullOrEmpty(line))
            {
                if (this.TextBlocks.Last().Lines.Any())
                {
                    this.TextBlocks.Add(new WebVttTextBlock());
                }

                continue;
            }

            var lastTextBlock = this.TextBlocks.Last();
            lastTextBlock.Lines.Add(new WebVttTextLine()
            {
                Text = line,
                LineNumber = lineNumber
            });
        }

        // Due to add new paragraph for new line, at file ending, there will be extra empty paragraph, so trim it.
        if (!this.TextBlocks.Last().Lines.Any())
        {
            this.TextBlocks.Remove(this.TextBlocks.Last());
        }
    }

    public async Task SaveAsync(string filepath)
    {
        if (string.IsNullOrEmpty(filepath))
        {
            throw new ArgumentNullException(nameof(filepath));
        }

        CommonHelper.EnsureFolderExistForFile(filepath);

        using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        using (var fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
        {
            foreach (var paragraph in this.TextBlocks)
            {
                await paragraph.WriteAsync(fileWriter).ConfigureAwait(false);
                await fileWriter.WriteLineAsync().ConfigureAwait(false);
            }
        }
    }
}
