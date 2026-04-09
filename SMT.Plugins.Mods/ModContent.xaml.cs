using SMT.Core;
using SMT.Core.Utilities;
using System.IO.Compression;

namespace SMT.Mods;

public partial class ModContent : ContentView
{
    private IMain _main;

	public ModContent(Mod mod, IMain main)
	{
		InitializeComponent();
        _main = main;

		Name.Text = mod.Name;
		Version.Text = $"Version: {mod.Version}";
		Author.Text = $"Author: {mod.Author}";

        using ZipArchive archive = ZipFile.OpenRead(mod.Path);
        ZipArchiveEntry? iconEntry = archive.GetEntry("icon.png");

        if (iconEntry != null )
        {
            ModIcon.Source = ImageSource.FromStream(() => iconEntry.Open());
        }

        InstallBtn.Command = new Command(async () =>
		{
            if (!Directory.Exists(Path.Combine(Core.SMT.TempDirectory, "base")))
            {
                await _main.DisplayAlertAsync("Decomp Not Found!", "Cannot find decompiled APK, try running Import", "Womp", "Womp");
                return;
            }

            await _main.EnableButtons([InstallBtn], false);
            await mod.InstallAsync();
            await _main.EnableButtons([InstallBtn], true);
        });
	}
}