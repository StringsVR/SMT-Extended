using SMT.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SMT.Core.Android.Runner
{
    public class ToolRunner
    {
        public event EventHandler<ToolEventArgs>? OnToolEvent;

        public string? WorkingDirectory { get; set; }

        private void RaiseEvent(ToolType tool, ToolEventType eventType, string? message = null)
        {
            OnToolEvent?.Invoke(this, new ToolEventArgs
            {
                Tool = tool,
                EventType = eventType,
                Message = message
            });

            // Logging is centralized in the core layer; this runner only raises events.
            CLogger.Info($"[{tool}][{eventType}]: {message}");
        }

        public async Task<bool> RunProcessAsync(
            string fileName,
            string args,
            ToolType tool,
            CancellationToken cancellationToken = default)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = WorkingDirectory ?? AppContext.BaseDirectory,
                },
                EnableRaisingEvents = true,
            };

            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    RaiseEvent(tool, ToolEventType.Output, e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    RaiseEvent(tool, ToolEventType.Error, e.Data);
            };

            process.Exited += (_, __) =>
            {
                try
                {
                    tcs.TrySetResult(process.ExitCode);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            };

            RaiseEvent(tool, ToolEventType.Started, $"{fileName} started");

            if (!process.Start())
                throw new InvalidOperationException($"Failed to start {fileName}");

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using (cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(true);
                }
                catch
                {
                }
            }))
            {
                int exitCode = await tcs.Task.ConfigureAwait(false);
                RaiseEvent(tool, ToolEventType.Completed, $"{fileName} finished (ExitCode: {exitCode})");
                return exitCode == 0;
            }
        }
    }
}
