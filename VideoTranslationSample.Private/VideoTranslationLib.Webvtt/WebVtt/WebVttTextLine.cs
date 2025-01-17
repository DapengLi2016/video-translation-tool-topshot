// <copyright file="WebVttTextLine.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice.WebVtt;

public class WebVttTextLine
{
    public string Text { get; set; }

    public int LineNumber { get; set; }

    public WebVttTextLine Clone()
    {
        return new WebVttTextLine()
        {
            Text = this.Text,
            LineNumber = this.LineNumber,
        };
    }
}
