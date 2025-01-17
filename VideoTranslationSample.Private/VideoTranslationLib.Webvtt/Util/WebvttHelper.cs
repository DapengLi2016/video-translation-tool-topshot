using System.Globalization;
using System;
using System.Text.RegularExpressions;
using Microsoft.SpeechServices.CustomVoice.VideoTranslation;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SpeechServices.VideoTranslationLib.PrivateBase;

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt;

public static class WebvttHelper
{
    public static async Task WriteStreamAsync(Stream stream, IEnumerable<VideoTranslationSubtitleItem> subtitleItems)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(subtitleItems);

        using TextWriter writer = new StreamWriter(stream, Encoding.UTF8);

        await writer.WriteLineAsync(VideoTranslationConst.FileHeaderText).ConfigureAwait(false);
        await writer.WriteLineAsync().ConfigureAwait(false);

        foreach (var subtitleItem in subtitleItems)
        {
            await writer.WriteLineAsync(subtitleItem.ToString()).ConfigureAwait(false);
            await writer.WriteLineAsync().ConfigureAwait(false);
        }
    }

    public static bool IsLineMatchOrStartWith(string line, string prefx)
    {
        if (string.IsNullOrEmpty(prefx))
        {
            throw new ArgumentNullException(nameof(prefx));
        }

        if (string.IsNullOrEmpty(line))
        {
            return false;
        }

        return string.Equals(line, prefx, StringComparison.Ordinal) || line.StartsWith($"{prefx} ", StringComparison.Ordinal);
    }

    public static string StandardizeTimeSpanText(string timeSpanText)
    {
        if (string.IsNullOrEmpty(timeSpanText))
        {
            return string.Empty;
        }

        if (Regex.Match(timeSpanText, "^[0-9]{2}:[0-9]{2}\\.[0-9]{3}").Success)
        {
            return $"00:{timeSpanText}";
        }

        return timeSpanText;
    }

    public static TimeSpan ParseTimeSpan(string timeSpanText)
    {
        if (string.IsNullOrEmpty(timeSpanText))
        {
            return TimeSpan.Zero;
        }

        var standardizedTimeSpanText = StandardizeTimeSpanText(timeSpanText);
        return TimeSpan.Parse(standardizedTimeSpanText, CultureInfo.InvariantCulture);
    }
}
