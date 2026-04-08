using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Utilities
{
    public class PathTools
    {
        public static void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Source not found: {sourceDir}");

            Directory.CreateDirectory(destinationDir);

            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(sourceDir, dirPath);
                Directory.CreateDirectory(Path.Combine(destinationDir, relative));
            }

            foreach (string filePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(sourceDir, filePath);
                string destFile = Path.Combine(destinationDir, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                File.Copy(filePath, destFile, true);
            }
        }

        public static void CopyDirectoryInDirectory(string inputDir, string outputDir)
        {
            if (!Directory.Exists(inputDir))
                throw new DirectoryNotFoundException($"Source not found: {inputDir}");


            foreach (string dirPath in Directory.GetDirectories(inputDir, "*", SearchOption.TopDirectoryOnly))
            {
                var relative = Path.GetRelativePath(inputDir, dirPath);
                var destDir = Path.Combine(outputDir, relative);
                CopyDirectory(dirPath, destDir);
            }
        }

        public static void ResetDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);
        }
    }
}
