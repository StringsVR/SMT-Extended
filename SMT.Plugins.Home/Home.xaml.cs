using CommunityToolkit.Maui.Storage;
using SMT.Core.Android.Runner;
using SMT.Core.Android.Tools;
using SMT.Core.Logging;
using SMT.Core.Utilities;

namespace SMT.Plugins.Home;

public partial class Home : ContentView
{
    private IMain _main;

    private ToolRunner _toolRunner;
    private Apktool _apktool;
    private ApkEditor _apkEditor;
    private UberSigner _signer;

	public Home(IMain main)
	{
		InitializeComponent();
        _main = main;
        _toolRunner = new();
        _apktool = new Apktool(_toolRunner);
        _apkEditor = new ApkEditor(_toolRunner);
        _signer = new UberSigner(_toolRunner);
	}

    private async void ImportBtn_Clicked(object sender, EventArgs e)
    {
        if (Directory.EnumerateDirectories(SMT.Core.SMT.BackupDirectory).Any())
        {
            bool useBackup = await _main.DisplayAlertAsync("Backup Directory Found!", "There are files in the backup directory. Would you like to use those?", "Yes", "No");
            if (useBackup)
                await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, false);
                await Task.Run(async () =>
                {
                    foreach (var directory in Directory.EnumerateDirectories(Core.SMT.TempDirectory))
                    {
                        CLogger.Info($"Found directory: {directory}");
                        if (directory.EndsWith("Backup", StringComparison.OrdinalIgnoreCase))
                        {
                            CLogger.Info($"Skipping 'Backup' directory: {directory}");
                            continue;
                        }

                        PathTools.DeleteDirectoryIfExists(directory);
                    }

                    PathTools.CopyDirectoryInDirectory(Core.SMT.BackupDirectory, Core.SMT.TempDirectory);
                });

            await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, true);
            CLogger.Info("Copied backup files to temp directory");
            return;
        }

        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select an APK",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".apks", ".zip" } }
                })
            });

            if (result == null)
                return;

            await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, false);
            await Task.Run(async () =>
            {
                await _apktool.Decompile(result.FullPath, Core.SMT.TempDirectory);

                foreach (var file in Directory.EnumerateFiles(Path.Combine(Core.SMT.TempDirectory, "unknown"), "*.apk", SearchOption.AllDirectories))
                {
                    CLogger.Info($"Decompiled file: {file}");
                    await _apktool.Decompile(file, Path.Combine(Core.SMT.BackupDirectory, Path.GetFileNameWithoutExtension(file)));
                }
            });
        }
        finally
        {
            PathTools.CopyDirectoryInDirectory(Core.SMT.BackupDirectory, Core.SMT.TempDirectory);
            PathTools.DeleteDirectoryIfExists(Path.Combine(Core.SMT.TempDirectory, "unknown"));
            PathTools.DeleteDirectoryIfExists(Path.Combine(Core.SMT.TempDirectory, "original"));
            PathTools.DeleteFileIfExists(Path.Combine(Core.SMT.TempDirectory, "apktool.yml"));
            await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, true);
        }
    }

    private async void CompileBtn_Clicked(object sender, EventArgs e)
    {
        if (!Directory.Exists(Path.Combine(Core.SMT.TempDirectory, "base")) || !Directory.Exists(Path.Combine(Core.SMT.TempDirectory, "split_asset_pack_install_time")))
        {
            await _main.DisplayAlertAsync("Decomp Not Found!", "Cannot find decompiled APK, try running Import", "Womp", "Womp");
            return;
        }

        try
        {
            string compiledPath = Path.Combine(Core.SMT.TempDirectory, "compiled");

            await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, false);
            List<String> apkNames = new List<String>();
            foreach (var dir in Directory.EnumerateDirectories(Core.SMT.BackupDirectory))
            {
                apkNames.Add(Path.GetFileName(dir));
            }

            foreach (var apk in apkNames)
            {
                await _apktool.Build(Path.Combine(Core.SMT.TempDirectory, apk), Path.Combine(compiledPath, $"{apk}.apk"));
            }

            string mergePath = Path.Combine(Core.SMT.TempDirectory, "compiled_merged.apk");
            PathTools.DeleteFileIfExists(mergePath);

            await _apkEditor.Merge(compiledPath);
            await _signer.Sign(mergePath, Path.Combine(compiledPath));

            var apkPath = Path.Combine(compiledPath, "compiled_merged-aligned-debugSigned.apk");
            using var stream = File.OpenRead(apkPath);
            var result = await FileSaver.Default.SaveAsync("modded.apk", stream);

            if (!result.IsSuccessful)
            {
                await _main.DisplayAlertAsync("Error", result.Exception?.Message ?? "Failed to save file", "OK", "OK");
            }
        }
        finally
        {
            await Task.Run(() => {
                foreach (var dir in Directory.EnumerateDirectories(Core.SMT.TempDirectory))
                {
                    if (Path.GetFileName(dir) == "Backup")
                        continue;

                    PathTools.DeleteDirectoryIfExists(dir);
                }

                PathTools.DeleteFileIfExists(Path.Combine(Core.SMT.TempDirectory, "compiled_merged.apk"));
            });


            await _main.EnableButtons(new List<Button> { ImportBtn, CompileBtn }, true);
        }
    }
}