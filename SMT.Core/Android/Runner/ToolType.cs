using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Android.Runner
{
    public enum ToolType
    {
        ApkTool,
        UberSigner,
        APKEditor,
        Other
    }

    public enum ToolEventType
    {
        Started,
        Output,
        Error,
        Completed
    }

    public class ToolEventArgs : EventArgs
    {
        public ToolType Tool { get; set; }
        public ToolEventType EventType { get; set; }
        public string? Message { get; set; }
    }
}
