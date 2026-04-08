using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        ContentView View { get; }
        void Initialize(IMain main);
    }
}
