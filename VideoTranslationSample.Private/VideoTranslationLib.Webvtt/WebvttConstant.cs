using System.Text.RegularExpressions;

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt;

public static class WebvttConstant
{
    public const string FileHeaderText = "WEBVTT";

    // 04:02.500 --> 04:05.000
    // 00:00:18.500 --> 00:00:20.500
    public static readonly Regex SubtitleTimelineMatch = new Regex("^(?<startTime>[0-9]{2}(:[0-9]{2}){1,2}(\\.[0-9]{3})?) --> (?<endTime>[0-9]{2}(:[0-9]{2}){1,2}(\\.[0-9]{3})?)");
}
