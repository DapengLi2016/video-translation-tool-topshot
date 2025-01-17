using System.Text.RegularExpressions;

namespace Microsoft.SpeechServices.VideoTranslationLib.Webvtt;

public static class TopshotConstant
{
    // 【代驾A】：そういえば、お前、大学生の女の子なのに
    public static readonly Regex SubtitleSpeakerExtractionMatch = new Regex("^【(?<speakerName>.+)】：(?<subtitle>.*)");
}
