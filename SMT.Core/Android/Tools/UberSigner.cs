using SMT.Core.Android.Runner;
using SMT.Core.Exceptions;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Android.Tools
{
    public class UberSigner : ITool
    {
        public ToolRunner _toolRunner { get; set; }
        public string _toolPath { get; set; }

        public UberSigner(ToolRunner toolRunner)
        {
            this._toolRunner = toolRunner;
            this._toolPath = Path.Combine(SMT.ToolsDirectory, "uber-apk-signer.jar");

            if (!File.Exists(_toolPath))
            {
                Task.Run(async () =>
                {
                    await DownloadTool();
                });
            }
        }

        public async Task Sign(string inputApkPath, string outputDir, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(inputApkPath))
                throw new ArgumentException("Input APK path must be provided.", nameof(inputApkPath));
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Output directory must be provided.", nameof(outputDir));

            string args = $"-jar \"{_toolPath}\" -a \"{inputApkPath}\" -o \"{outputDir}\"";
            bool success = await _toolRunner.RunProcessAsync("java", args, ToolType.UberSigner, ct).ConfigureAwait(false);
            if (!success)
                throw new ToolExecutionException(ToolType.UberSigner, 1, $"UberSigner failed on {inputApkPath}.");
        }

        public bool ToolExists()
        {
            return File.Exists(_toolPath);
        }
        public async Task DownloadTool()
        {
            await NetworkUtils.DownloadFileAsync("https://github.com/patrickfav/uber-apk-signer/releases/download/v1.3.0/uber-apk-signer-1.3.0.jar", _toolPath);
        }
    }
}
