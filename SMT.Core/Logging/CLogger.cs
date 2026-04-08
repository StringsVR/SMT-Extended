using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Logging
{
    public static class CLogger
    {
        private static readonly string LogFilePath;

        static CLogger()
        {

            // Ensure the Logs directory exists
            if (!Directory.Exists(SMT.LogsDirectory))
                Directory.CreateDirectory(SMT.LogsDirectory);

            // Create a timestamped log file
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            LogFilePath = Path.Combine(SMT.LogsDirectory, $"log_{timestamp}.txt");
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warning(string message)
        {
            Write("WARNING", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(logMessage); // optional: prints to console
            try
            {
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }
}
