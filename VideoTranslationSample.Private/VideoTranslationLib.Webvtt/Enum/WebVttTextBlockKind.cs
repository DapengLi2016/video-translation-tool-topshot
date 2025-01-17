// <copyright file="WebVttTextBlockKind.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslationLib.PublicBase.Enum;

public enum WebVttTextBlockKind
{
    None = 0,

    Header,

    VideoTranslationFileNote,

    VideoTranslationParagraphNote,

    OtherNote,

    /* for example:
        00:00:25.250 --> 00:00:26.477
        The pretending to be Batman.
    */
    Subtitle,

    /* for example:
        3
        00:00:25.250 --> 00:00:26.477
        The pretending to be Batman.
    */
    SubtitleWithParagraphOrder,

    Unknown,
}
