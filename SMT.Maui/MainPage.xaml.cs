using SMT.Core.Logging;
using SMT.Core.Plugins;
using SMT.Core.Utilities;

namespace SMT.Maui
{
    public partial class MainPage : ContentPage, IMain
    {
        private void CreateSidePanelBtn(IPlugin plugin)
        {
            VerticalContent.Add(new Button
            {
                Text = plugin.Name,
                BackgroundColor = Color.FromArgb("#282828"),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                Command = new Command(() =>
                {
                    MainContent.Content = plugin.View;
                })
            });
        }

        Task<bool> IMain.DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            return DisplayAlertAsync(title, message, accept, cancel);
        }

        async Task IMain.EnableButtons(List<Button> btns, bool en)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (var btn in btns)
                    btn.IsEnabled = en;

                foreach (var child in VerticalContent.Children)
                    if (child is Button button)
                        button.IsEnabled = en;
            });
        }

        public MainPage()
        {
            CLogger.Info("SMT Initialized");
            SMT.Core.SMT.InitializeDirectories();
            InitializeComponent();

            var plugins = PluginLoader.LoadPlugins(this, SMT.Core.SMT.PluginsDirectory);
            if (plugins.Count > 0)
                MainContent.Content = plugins[0].View;

            foreach (var plugin in plugins)
            {
                CLogger.Info($"Loaded Plugin {plugin.Name}");
                CreateSidePanelBtn(plugin);
            }
        }
    }
}
