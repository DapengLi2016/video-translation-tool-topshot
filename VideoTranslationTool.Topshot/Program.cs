//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using CommandLine;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.VideoTranslationSample.PrivatePreview;
using Microsoft.SpeechServices.VideoTranslationTool.Topshot.Handlers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        var types = LoadVerbs();

        var exitCode = await Parser.Default.ParseArguments(args, types)
            .MapResult(
                options => RunAndReturnExitCodeAsync(options),
                _ => Task.FromResult(1));

        if (exitCode == 0)
        {
            Console.WriteLine("Process completed successfully.");
        }
        else
        {
            Console.WriteLine($"Failure with exit code: {exitCode}");
        }

        return exitCode;
    }

    static async Task<int> RunAndReturnExitCodeAsync(object options)
    {
        ArgumentNullException.ThrowIfNull(options);
        try
        {
            return await DoRunAndReturnExitCodeAsync(options).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to run with exception: {e.Message}");
            return CommonPublicConst.ExistCodes.GenericError;
        }
    }

    static async Task<int> DoRunAndReturnExitCodeAsync(object baseOptions)
    {
        ArgumentNullException.ThrowIfNull(baseOptions);

        switch (baseOptions)
        {
            case ConvertSubtitleFileToJsonWebvttOptions options:
                {
                    await ConvertSubtitleToJsonWebvttHandler.ConvertSubtitleFileToJsonWebvttAsync(
                        sourceSubtitleFilePath: options.SourceSubtitleFilePath,
                        targetWebvttFilePath: options.TargetWebvttFilePath).ConfigureAwait(false);
                    break;
                }

            case ConvertSubtitleDirToJsonWebvttOptions options:
                {
                    await ConvertSubtitleToJsonWebvttHandler.ConvertSubtitleDirToJsonWebvttAsync(
                        sourceSubtitleDirPath: options.SourceSubtitleDirPath,
                        targetWebvttDirPath: options.TargetWebvttDirPath).ConfigureAwait(false);
                    break;
                }

            default:
                throw new NotSupportedException();
        }


        return CommonPublicConst.ExistCodes.NoError;
    }

    //load all types using Reflection
    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}