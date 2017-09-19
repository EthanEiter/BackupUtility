using Backup.Utility.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backup.Utility.Core
{
    public static class SettingsManager
    {
        public static string LogsPath => Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Backup.Utility\logs");

        public static bool CloseConsole
        {
            get => (bool)Settings.Default["CloseConsole"];
            set
            {
                Settings.Default["CloseConsole"] = value;
                Settings.Default.Save();
            }
        }

        public static bool PathVisibility
        {
            get => (bool)Settings.Default["PathVisibility"];
            set
            {
                Settings.Default["PathVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static bool LogVisibility
        {
            get => (bool)Settings.Default["LogVisibility"];
            set
            {
                Settings.Default["LogVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static bool DrivesVisibility
        {
            get => (bool)Settings.Default["DrivesVisibility"];
            set
            {
                Settings.Default["DrivesVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static bool Clone
        {
            get => (bool)Settings.Default["Clone"];
            set
            {
                Settings.Default["Clone"] = value;
                Settings.Default.Save();
            }
        }

        public static string BackupPath
        {
            get => Settings.Default["BackupPath"].ToString();
            set
            {
                Settings.Default["BackupPath"] = value;
                Settings.Default.Save();
            }
        }

        public static string BackupSource
        {
            get => Settings.Default["BackupSource"].ToString();
            set
            {
                Settings.Default["BackupSource"] = value;
                Settings.Default.Save();
            }
        }

        public static List<string> LogList
        {
            get
            {
                var list = new List<string>();
                if(Directory.Exists(LogsPath))
                    foreach(var file in Directory.GetFiles(LogsPath))
                    {
                        list.Add(Path.GetFileNameWithoutExtension(file));
                    }
                return list;
            }
        }

        public static string GetLog(string logName)
        {
            var lines = File.ReadLines(Path.Combine(LogsPath, $"{logName}.txt")).Reverse().Take(15).Reverse();
            var log = string.Empty;
            foreach (var line in lines)
            {
                log += $"{line}\n";
            }
            return log;
        }
    }
}
