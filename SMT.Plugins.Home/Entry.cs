using SMT.Core.Plugins;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Plugins.Home
{
    public class Entry : IPlugin
    {
        public required IMain Main { get; set; }

        public string Name => "Home";
        public ContentView View {  get; set; }

        public void Initialize(IMain main)
        {

            Main = main;
            View = new Home(Main);
        }
    }
}