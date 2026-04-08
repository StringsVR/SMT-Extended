using SMT.Core.Logging;
using SMT.Core.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMT.Core
{
    public class Mod
    {
        public sealed record ModMetadata(string Name, string Author, string Version);

        private readonly ModMetadata _metadata;
        private readonly string _archivePath;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public string Name => _metadata.Name;
        public string Version => _metadata.Version;
        public string Author => _metadata.Author;
        public string Path => _archivePath;

        public Mod(ModMetadata metadata, string archivePath)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _archivePath = archivePath ?? throw new ArgumentNullException(nameof(archivePath));

            CLogger.Info($"Mod created: {Name} (Version: {Version}, Author: {Author})");
        }

        public async Task InstallAsync()
        {
            string modTemp = System.IO.Path.Combine(SMT.TempDirectory, "Mods");
            string gameBase = System.IO.Path.Combine(SMT.TempDirectory, "base");
            string gameSplit = System.IO.Path.Combine(SMT.TempDirectory, "split_asset_pack_install_time");

            // Reset temp directory
            PathTools.ResetDirectory(modTemp);

            await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(_archivePath);
                archive.ExtractToDirectory(modTemp, overwriteFiles: true);

                string basePath = System.IO.Path.Combine(modTemp, "base");
                string splitPath = System.IO.Path.Combine(modTemp, "split");

                if (Directory.Exists(basePath))
                    PathTools.CopyDirectory(basePath, gameBase);

                if (Directory.Exists(splitPath))
                    PathTools.CopyDirectory(splitPath, gameSplit);

                CLogger.Info($"Mod '{Name}' installed successfully.");
            });

            PathTools.DeleteDirectoryIfExists(modTemp);
        }

        public static Mod? LoadFromArchive(string archivePath)
        {
            if (string.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("Path cannot be empty.", nameof(archivePath));

            if (!File.Exists(archivePath))
                throw new FileNotFoundException($"Mod archive not found: {archivePath}", archivePath);

            using var archive = ZipFile.OpenRead(archivePath);
            var metaEntry = archive.GetEntry("meta.json");

            if (metaEntry == null)
                return null;

            ModMetadata? metadata;
            using (var reader = new StreamReader(metaEntry.Open(), Encoding.UTF8))
            {
                string content = reader.ReadToEnd();
                try
                {
                    metadata = JsonSerializer.Deserialize<ModMetadata>(content, JsonOptions);
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException($"Failed to parse mod metadata in '{System.IO.Path.GetFileName(archivePath)}': {ex.Message}", ex);
                }
            }

            if (metadata == null || string.IsNullOrWhiteSpace(metadata.Name))
                throw new InvalidDataException($"Mod metadata in '{System.IO.Path.GetFileName(archivePath)}' is empty or invalid.");

            metadata = metadata with
            {
                Author = string.IsNullOrWhiteSpace(metadata.Author) ? "Unknown" : metadata.Author,
                Version = string.IsNullOrWhiteSpace(metadata.Version) ? "Unknown" : metadata.Version
            };

            return new Mod(metadata, archivePath);
        }
    }
}