using SMT.Core.Android.Runner;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Android.Tools
{
    public interface ITool
    {
        ToolRunner _toolRunner { get; set; }
        string _toolPath { get; set; }
        Task DownloadTool();
        bool ToolExists();
    }
}
