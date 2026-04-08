using SMT.Core.Android.Runner;
using SMT.Core.Exceptions;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Android.Tools
{
    public class ApkEditor : ITool
    {
        public ToolRunner _toolRunner { get; set; }
        public string _toolPath { get; set; }

        public ApkEditor(ToolRunner toolRunner)
        {
            _toolRunner = toolRunner;
            _toolPath = Path.Combine(SMT.ToolsDirectory, "APKEditor.jar");

            if (!File.Exists(_toolPath))
            {
                Task.Run(async () =>
                {
                    await DownloadTool();
                });
            }
        }

        public async Task Merge(string inputApkPath, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(inputApkPath))
                throw new ArgumentException("Input APK path must be provided.", nameof(inputApkPath));

            string args = $"-jar \"{_toolPath}\" m -i \"{inputApkPath}\"";
            bool success = await _toolRunner.RunProcessAsync("java", args, ToolType.APKEditor, ct).ConfigureAwait(false);
            if (!success)
                throw new ToolExecutionException(ToolType.APKEditor, 1, $"ApkEditor failed on {inputApkPath}.");
        }

        public bool ToolExists()
        {
            return File.Exists(_toolPath);
        }

        public async Task DownloadTool()
        {
            await NetworkUtils.DownloadFileAsync("https://github.com/REAndroid/APKEditor/releases/download/V1.4.8/APKEditor-1.4.8.jar", this._toolPath);
        }
    }
}
