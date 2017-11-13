using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backup.Utility.Core
{
    public static class LogManager
    {
        public static string LogsPath =>
            Path.Combine(Environment.GetEnvironmentVariable("LocalAppData") ?? throw new InvalidOperationException(),
                @"Backup.Utility\logs");

        public static string GetLog(string logName)
        {
            var lines = File.ReadLines(Path.Combine(LogsPath, $"{logName}.txt")).Reverse().Take(15).Reverse();
            return lines.Aggregate(string.Empty, (current, line) => current + $"{line}\n");
        }

        public static List<string> LogList
        {
            get
            {
                if (!Directory.Exists(LogsPath))
                {
                    Directory.CreateDirectory(LogsPath);
                }
                return Directory.GetFiles(LogsPath).Select(Path.GetFileNameWithoutExtension).ToList();
            }
        }

        public static string GetFullLogPath(string name, Functions function) =>
            $"{Path.Combine(LogsPath, GetLogName(name, function))}.txt";

        public static string GetLogName(string name, Functions function, bool fixName = false) =>
            fixName
                ? $"{NameFixer(name)}_{function.ToString().ToLower()}_log"
                : $"{name}_{function.ToString().ToLower()}_log";

        public static string NameFixer(string name) =>
            string.IsNullOrEmpty(name) ? "UNKNOWN" : name.Replace(' ', '_');
    }
}
