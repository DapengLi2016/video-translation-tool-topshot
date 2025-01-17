// <copyright file="VideoFilePortalProperties.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;

using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using System;

public class VideoFilePortalProperties : StatelessResourceBase
{
    public Uri VocalsAudioFileUri { get; set; }
}
