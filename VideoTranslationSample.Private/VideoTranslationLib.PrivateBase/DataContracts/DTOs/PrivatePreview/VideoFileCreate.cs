//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;

using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using System;
using System.Globalization;

public class VideoFileCreate : StatelessResourceBase
{
    public CultureInfo Locale { get; set; }

    public int? SpeakerCount { get; set; }

    // by default MultipartFormFile
    public VideoTranslationFileUploadKind? UploadKind { get; set; }

    public Uri VideoOrAudioFileUrl { get; set; }

    public string VideoOrAudioFileNameExtension { get; set; }

    public TimeSpan? EstimatedDuration { get; set; }

    // When ApiVersion is null or < 2, Status and LastActionDateTime is null for compatiable.
    // When ApiVersion >= 2, response Status and LastActionDateTime
    public int? ApiVersion { get; set; }
}
