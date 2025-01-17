// <copyright file="VideoTranslationWebvttFileKind.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Common.Client;

public enum VideoTranslationWebvttFileKind
{
    None = 0,

    SourceLocaleSubtitle,

    TargetLocaleSubtitle,

    MetadataJson,
}
