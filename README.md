# Video translation

Video translation client tool for Topshot

# Supported OS
## Windows prerequisite:
   Install [dotnet 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
   
   Run tool: VideoTranslationTool.Topshot.exe [verb] [arguments]

## Linux prerequisite:
   Install [dotnet 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

   Run tool: dotnet VideoTranslationTool.Topshot.exe [verb] [arguments]

# Command Line Usage
   | Description | Command line arguments |
   | ------------ | -------------- |
   | Convert subtitle to metadata json webvtt file. | convertSubtitleToJsonWebvtt --sourceSubtitleFilePath [localSourceSubtitleFilePath](TestData/ConvertSubtitleToJsonWebvtt/Input.vtt) --targetWebvttFilePath [localTargetWebvttFilePath](TestData/ConvertSubtitleToJsonWebvtt/Output.vtt)  |
   | Convert subtitle to metadata json webvtt file. | convertSubtitleDirToJsonWebvtt --sourceSubtitleDirPath [localSourceSubtitleDirPath] --targetWebvttDirPath [localTargetWebvttDirPath]  |

# Command line tool arguments
   | Verb | Argument | Is Required | Supported Values Sample | Description |
   | -------- | -------- | -------- | ---------------- | ----------- |
   | convertSubtitleToJsonWebvtt | --sourceSubtitleFilePath  | True | C:\hello.vtt | Local file path |
   | convertSubtitleToJsonWebvtt | --targetWebvttFilePath | True | C:\hello.vtt | Local file path |
   | convertSubtitleToJsonWebvtt | --sourceSubtitleDirPath  | True | C:\sourceDir | Local dir path |
   | convertSubtitleToJsonWebvtt | --targetWebvttDirPath | True | C:\targetDir | Local dir path |

