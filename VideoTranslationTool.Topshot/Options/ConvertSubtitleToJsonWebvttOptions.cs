//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PrivatePreview;

using CommandLine;
using System;

[Verb("convertSubtitleToJsonWebvtt", HelpText = "convert .")]
public class ConvertSubtitleToJsonWebvttOptions
{
    [Option("sourceSubtitleFilePath", Required = true, HelpText = "Specify subtitle file path.")]
    public string SourceSubtitleFilePath { get; set; }

    [Option("targetWebvttFilePath", Required = true, HelpText = "Specify target webvtt metadata json file path.")]
    public string TargetWebvttFilePath { get; set; }
}

