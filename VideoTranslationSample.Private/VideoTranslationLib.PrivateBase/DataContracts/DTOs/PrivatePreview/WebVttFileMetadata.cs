﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs.FileEntity;

using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using System;
using System.Globalization;

public class WebVttFileMetadata : StatelessResourceBase
{
    public CultureInfo Locale { get; set; }

    public Uri FileUrl { get; set; }
}
