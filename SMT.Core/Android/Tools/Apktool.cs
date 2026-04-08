using SMT.Core.Android.Runner;
using SMT.Core.Exceptions;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Android.Tools
{
    public class Apktool : ITool
    {
        public ToolRunner _toolRunner {get; set;}
        public string _toolPath { get; set; }

        public Apktool(ToolRunner toolRunner)
        {
            _toolRunner = toolRunner;
            this._toolPath = Path.Combine(SMT.ToolsDirectory, "apktool.bat");

            if (!File.Exists(_toolPath))
            {
                Task.Run(async () =>
                {
                    await DownloadTool();
                });
            }
        }

        public async Task Decompile(string path, string output, CancellationToken ct = default)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not found!");

            bool success = await _toolRunner.RunProcessAsync(_toolPath, $"d \"{path}\" -o \"{output}\" -f", ToolType.ApkTool, ct);
            if (!success)
                throw new ToolExecutionException(ToolType.ApkTool, 1, $"Apktool failed on {path}.");
        }

        public async Task Build(string path, string output, CancellationToken ct = default)
        {
            if (!Directory.Exists(path))
                throw new ToolExecutionException(ToolType.ApkTool, 1, $"{path} not found!");

            bool success = await _toolRunner.RunProcessAsync(_toolPath, $"b \"{path}\" -o \"{output}\" -f", ToolType.ApkTool, ct);
            if (!success)
                throw new ToolExecutionException(ToolType.ApkTool, 1, $"Apktool failed on {path}.");
        }

        public bool ToolExists()
        {
            return File.Exists(_toolPath);
        }

        public async Task DownloadTool()
        {
            string batPath = this._toolPath;
            string jarPath = Path.Combine(SMT.ToolsDirectory, "apktool_2.9.3.jar");

            // Download files
            await NetworkUtils.DownloadFileAsync(
                "https://raw.githubusercontent.com/iBotPeaches/Apktool/master/scripts/windows/apktool.bat",
                batPath);

            await NetworkUtils.DownloadFileAsync(
                "https://bitbucket.org/iBotPeaches/apktool/downloads/apktool_2.9.3.jar",
                jarPath);

            // Remove last line from apktool.bat
            if (File.Exists(batPath))
            {
                var lines = await File.ReadAllLinesAsync(batPath);

                if (lines.Length > 0)
                {
                    // Remove last line
                    var trimmed = lines[..^1]; // C# range operator

                    await File.WriteAllLinesAsync(batPath, trimmed);
                }
            }
        }
    }
}
