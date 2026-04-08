using SMT.Core.Logging;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core
{
    public class SMT
    {
        public const string Version = "1.0.4";

        public static string BaseDirectory => AppContext.BaseDirectory;
        public static string TempDirectory => Path.Combine(BaseDirectory, "Temp");
        public static string BackupDirectory => Path.Combine(TempDirectory, "Backup");
        public static string LogsDirectory => Path.Combine(BaseDirectory, "Logs");
        public static string ModsDirectory => Path.Combine(BaseDirectory, "Mods");
        public static string ToolsDirectory => Path.Combine(BaseDirectory, "Tools");
        public static string PluginsDirectory => Path.Combine(BaseDirectory, "Plugins");

        public static void InitializeDirectories()
        {
            CLogger.Info("Ensuring Necessary Directories");
            PathTools.CreateDirectoryIfNotExists(TempDirectory);
            PathTools.CreateDirectoryIfNotExists(BackupDirectory);
            PathTools.CreateDirectoryIfNotExists(LogsDirectory);
            PathTools.CreateDirectoryIfNotExists(ModsDirectory);
            PathTools.CreateDirectoryIfNotExists(ToolsDirectory);
            PathTools.CreateDirectoryIfNotExists(PluginsDirectory);
        }
    }
}
