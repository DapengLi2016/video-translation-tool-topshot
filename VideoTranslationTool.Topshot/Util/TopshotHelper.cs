
namespace Microsoft.SpeechServices.VideoTranslationTool.Topshot.Util;

using Microsoft.SpeechServices.VideoTranslationLib.Webvtt;

internal class TopshotHelper
{
    public static (string speakerId, string subtitle) IsMatchSubtitleTimeFormat(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return (null, line);
        }

        var match = TopshotConstant.SubtitleSpeakerExtractionMatch.Match(line);
        if (!match.Success || !match.Groups.ContainsKey("speakerName"))
        {
            return (null, line);
        }

        var speakerName = match.Groups["speakerName"].Value;
        var subtitle = string.Empty;
        if (match.Groups.ContainsKey("subtitle"))
        {
            subtitle = match.Groups["subtitle"].Value;
        }

        return (speakerName, subtitle);
    }
}
