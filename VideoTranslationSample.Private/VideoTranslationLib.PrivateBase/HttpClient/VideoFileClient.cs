//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.Public.Enums;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.CustomVoice.TtsLib.Util;
using Microsoft.SpeechServices.DataContracts;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs.FileEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class VideoFileClient : HttpClientBase
{
    public VideoFileClient(HttpClientConfigBase config)
        : base(config)
    {
    }

    public override string ControllerName => "videofiles";

    public async Task<VideoFileMetadata> UploadVideoFileAsync(
        string description,
        CultureInfo locale,
        int? speakerCount,
        string videoFilePath,
        Uri videoFileUrl,
        string videoOrAudioFileNameExtension)
    {
        return await RequestWithRetryAsync(async () =>
        {
            var response = PostUploadVideoFileWithResponseAsync(
                description: description,
                locale: locale,
                speakerCount: speakerCount,
                videoFilePath: videoFilePath,
                videoFileUrl: videoFileUrl,
                videoOrAudioFileNameExtension: videoOrAudioFileNameExtension);

            return await response
                .ReceiveJson<VideoFileMetadata>();
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<VideoFileMetadata>> QueryVideoFilesAsync()
    {
        var url = BuildRequestBase()
            // Set apiVersion to 2 to response status.
            .SetQueryParam("apiVersion", "2");

        return await RequestWithRetryAsync(async () =>
        {
            return await url.GetAsync()
                .ReceiveJson<PaginatedResources<VideoFileMetadata>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<IFlurlResponse> DeleteVideoFileAsync(
        Guid videoFileId,
        bool deleteAssociations)
    {
        var queryParams = new Dictionary<string, string>();
        if (deleteAssociations)
        {
            queryParams["deleteAssociations"] = bool.TrueString;
        }

        return await RequestWithRetryAsync(async () =>
        {
            return await DeleteByIdAsync(
                id: videoFileId.ToString(),
                queryParams: queryParams).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<Translation>> QueryTranslationsAsync(Guid videoFileId)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(videoFileId.ToString())
            .AppendPathSegment("translations");

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetAsync()
                .ReceiveJson<PaginatedResources<Translation>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<WebVttFileMetadata> QueryTargetLocaleWebVttAsync(
        Guid videoFileId,
        CultureInfo locale)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(videoFileId.ToString())
            .AppendPathSegment(locale.Name)
            .AppendPathSegment("webvtt");

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetAsync()
                .ReceiveJson<WebVttFileMetadata>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<VideoFileMetadata> QueryVideoFileWithLocaleAndFileContentSha256Async(
        CultureInfo locale,
        string fileContentSha256)
    {
        if (locale == null || string.IsNullOrEmpty(locale.Name))
        {
            throw new ArgumentNullException(nameof(locale));
        }

        if (string.IsNullOrEmpty(fileContentSha256))
        {
            throw new ArgumentNullException(nameof(fileContentSha256));
        }

        var url = BuildRequestBase()
            .AppendPathSegment("QueryByFileContentSha256")
            .SetQueryParam("locale", locale.Name)
            .SetQueryParam("fileContentSha256", fileContentSha256)
            // Set apiVersion to 2 to response status.
            .SetQueryParam("apiVersion", "2");

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ReceiveJson<VideoFileMetadata>()
                    .ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }

                Console.Write($"Response failed with error: {await ex.GetResponseStringAsync().ConfigureAwait(false)}");
                throw;
            }
        }).ConfigureAwait(false);
    }

    // Set apiVersion to 2 to response status.
    public async Task<VideoFileMetadata> QueryVideoFileAsync(
        Guid id,
        int? apiRequestVersion = 2)
    {
        var response = await this.QueryVideoFileWithResponseAsync(id, apiRequestVersion).ConfigureAwait(false);
        return await response.GetJsonAsync<VideoFileMetadata>().ConfigureAwait(false);
    }

    public async Task<IFlurlResponse> QueryVideoFileWithResponseAsync(
        Guid id,
        int? apiRequestVersion = 2)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(id.ToString());
        if (apiRequestVersion != null)
        {
            url = url.SetQueryParam("apiVersion", apiRequestVersion.Value.ToString(CultureInfo.InvariantCulture));
        }

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetAsync()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<VideoFileMetadata> UploadVideoFileAndWaitUntilTerminatedAsync(
        CultureInfo sourceLocale,
        int? speakerCount,
        string videoFilePath,
        Uri videoFileAzureBlobUrl,
        string videoOrAudioFileNameExtension)
    {
        if (!string.IsNullOrWhiteSpace(videoFilePath))
        {
            if (!File.Exists(videoFilePath))
            {
                throw new FileNotFoundException(videoFilePath);
            }

            Console.WriteLine($"Uploading file: {videoOrAudioFileNameExtension}");
        }
        else if (!string.IsNullOrWhiteSpace(videoFileAzureBlobUrl?.OriginalString))
        {
            var (success, error) = await CommonHttpClientHelper.IsUriExistAsync(videoFileAzureBlobUrl).ConfigureAwait(false);
            if (!success)
            {
                throw new InvalidDataException($"Url is not exist: {videoFileAzureBlobUrl}");
            }

            Console.WriteLine($"Uploading file: {videoFileAzureBlobUrl}");
        }

        var videoOrAudioFile = await this.UploadVideoFileAsync(
            description: null,
            locale: sourceLocale,
            speakerCount: speakerCount,
            videoFilePath: videoFilePath,
            videoFileUrl: videoFileAzureBlobUrl,
            videoOrAudioFileNameExtension: videoOrAudioFileNameExtension).ConfigureAwait(false);
        Console.WriteLine(JsonConvert.SerializeObject(
            videoOrAudioFile,
            Formatting.Indented,
            CommonPublicConst.Json.WriterSettings));
        if (!new[] { OneApiState.Succeeded, OneApiState.Failed }.Contains(videoOrAudioFile?.Status ?? OneApiState.Succeeded))
        {
            videoOrAudioFile = await this.QueryVideoFileUntilTerminatedAsync(
                id: videoOrAudioFile.ParseIdFromSelf()).ConfigureAwait(false);
            if (videoOrAudioFile == null)
            {
                throw new InvalidDataException($"failed to import video file");
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(
            videoOrAudioFile,
            Formatting.Indented,
            CommonPublicConst.Json.WriterSettings));
        return videoOrAudioFile;
    }

    public async Task<VideoFileMetadata> QueryVideoFileUntilTerminatedAsync(Guid id)
    {
        var startTime = DateTime.Now;
        VideoFileMetadata videoFile = null;
        OneApiState? lastState = null;
        do
        {
            videoFile = await this.QueryVideoFileAsync(id).ConfigureAwait(false);
            if (videoFile?.Status == null)
            {
                return videoFile;
            }

            if (videoFile.Status != lastState)
            {
                Console.WriteLine(videoFile.Status.Value);
                lastState = videoFile.Status;
            }

            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            Console.Write(".");
        }
        while (!new[] { OneApiState.Succeeded, OneApiState.Failed }.Contains(videoFile.Status.Value) &&
            DateTime.Now - startTime < CommonPublicConst.Http.LongRunOperationTaskExpiredDuration);
        return videoFile;
    }

    public async Task<IFlurlResponse> PostUploadVideoFileWithResponseAsync(
        string description,
        CultureInfo locale,
        int? speakerCount,
        string videoFilePath,
        Uri videoFileUrl,
        string videoOrAudioFileNameExtension,
        int? apiRequestVersion = 2)
    {
        if (string.IsNullOrEmpty(videoFilePath) && string.IsNullOrWhiteSpace(videoFileUrl?.OriginalString))
        {
            throw new ArgumentException($"Please provide either {nameof(videoFilePath)} or {nameof(videoFileUrl)}");
        }

        var name = string.Empty;
        if (!string.IsNullOrWhiteSpace(videoFilePath))
        {
            name = Path.GetFileName(videoFilePath);
        }
        else if (!string.IsNullOrWhiteSpace(videoFileUrl?.OriginalString))
        {
            name = UriHelper.GetFileName(videoFileUrl);
        }

        var url = BuildRequestBase();

        url.ConfigureRequest(settings => settings.Timeout = CommonPublicConst.Http.UploadFileTimeout);

        Console.WriteLine(url.Url);
        return await url
            .PostMultipartAsync(mp =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    mp.AddString(nameof(VideoFileCreate.DisplayName), name);
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    mp.AddString(nameof(VideoFileCreate.Description), description);
                }

                if (locale != null && !string.IsNullOrEmpty(locale.Name))
                {
                    mp.AddString(nameof(VideoFileCreate.Locale), locale.Name);
                }

                if (speakerCount != null)
                {
                    mp.AddString(nameof(VideoFileCreate.SpeakerCount), speakerCount.Value.ToString(CultureInfo.InvariantCulture));
                }

                // When ApiVersion is null or < 2, for video file response, Status and LastActionDateTime is null for compatiable.
                // When ApiVersion >= 2, response Status and LastActionDateTime
                if (apiRequestVersion != null)
                {
                    mp.AddString("apiVersion", apiRequestVersion.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (!string.IsNullOrWhiteSpace(videoFilePath))
                {
                    mp.AddFile("videoFile", videoFilePath);
                }
                else if (!string.IsNullOrWhiteSpace(videoFileUrl?.OriginalString))
                {
                    mp.AddString(nameof(VideoFileCreate.UploadKind), VideoTranslationFileUploadKind.AzureBlobUrl.ToString());
                    mp.AddString(nameof(VideoFileCreate.VideoOrAudioFileUrl), videoFileUrl.OriginalString);
                    if (!string.IsNullOrWhiteSpace(videoOrAudioFileNameExtension))
                    {
                        mp.AddString(nameof(VideoFileCreate.VideoOrAudioFileNameExtension), videoOrAudioFileNameExtension);
                    }
                }
            });
    }
}
