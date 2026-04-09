using SMT.Core;
using SMT.Core.Logging;
using SMT.Core.Utilities;
using SMT.Mods;

namespace SMT.Plugins.Mods;

public partial class Mods : ContentView
{
    private IMain _main;

    private void RefreshMods()
    {
        ModContentGrid.Children.Clear();
        foreach (var modPath in Directory.GetFiles(Core.SMT.ModsDirectory))
        {
            CLogger.Info($"Found .ykm at {modPath}");
            var mod = Mod.LoadFromArchive(modPath);
            if (mod == null)
                continue;

            ModContentGrid.Add(new ModContent(mod, _main));
        }
    }

    public Mods(IMain main)
	{
		InitializeComponent();
        _main = main;

        RefreshMods();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select an Yo-Kai Watch Mod",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".ykm", ".zip" } }
            })
        });

        if (result == null)
            return;

        await Task.Run(() =>
        {
            File.Copy(result.FullPath, Path.Combine(Core.SMT.ModsDirectory, result.FileName), true);
        });

        RefreshMods();
    }
}