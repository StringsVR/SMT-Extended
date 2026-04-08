using SMT.Core.Plugins;
using SMT.Core.Utilities;

namespace SMT.Plugins.Mods
{
    // All the code in this file is included in all platforms.
    public class Entry : IPlugin
    {
        public required IMain Main { get; set; }

        public string Name => "Mods";
        public ContentView View { get; set; }

        public void Initialize(IMain main)
        {

            Main = main;
            View = new Mods(Main);
        }
    }
}
