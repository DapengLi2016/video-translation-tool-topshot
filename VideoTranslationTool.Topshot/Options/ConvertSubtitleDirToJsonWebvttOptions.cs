//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PrivatePreview;

using CommandLine;
using System;

[Verb("convertSubtitleDirToJsonWebvtt", HelpText = "Convert subtitle files in dir to json webvtt .")]
public class ConvertSubtitleDirToJsonWebvttOptions
{
    [Option("sourceSubtitleDirPath", Required = true, HelpText = "Specify subtitle dir path.")]
    public string SourceSubtitleDirPath { get; set; }

    [Option("targetWebvttDirPath", Required = true, HelpText = "Specify target webvtt metadata json dir path.")]
    public string TargetWebvttDirPath { get; set; }
}

