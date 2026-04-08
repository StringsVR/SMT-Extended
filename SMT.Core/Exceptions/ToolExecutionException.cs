using SMT.Core.Android.Runner;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Exceptions
{
    public class ToolExecutionException : Exception
    {
        public ToolType Tool { get; }

        public int ExitCode { get; }

        public ToolExecutionException(ToolType tool, int exitCode, string message)
            : base(message)
        {
            Tool = tool;
            ExitCode = exitCode;
        }
    }
}
