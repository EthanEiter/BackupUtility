using Backup.Utility.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace Backup.Utility.Core
{
    public static class SettingsManager
    {
        public static string LogsPath => Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Backup.Utility\logs");

        public static bool CloseConsole
        {
            get { return (bool)Settings.Default["CloseConsole"]; }
            set
            {
                Settings.Default["CloseConsole"] = value;
                Settings.Default.Save();
            }
        }

        public static bool PathVisibility
        {
            get { return (bool)Settings.Default["PathVisibility"]; }
            set
            {
                Settings.Default["PathVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static bool LogVisibility
        {
            get { return (bool)Settings.Default["LogVisibility"]; }
            set
            {
                Settings.Default["LogVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static bool DrivesVisibility
        {
            get { return (bool)Settings.Default["DrivesVisibility"]; }
            set
            {
                Settings.Default["DrivesVisibility"] = value;
                Settings.Default.Save();
            }
        }

        public static string BackupPath
        {
            get { return Settings.Default["BackupPath"].ToString(); }
            set
            {
                Settings.Default["BackupPath"] = value;
                Settings.Default.Save();
            }
        }

        public static string BackupSource
        {
            get { return Settings.Default["BackupSource"].ToString(); }
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
                foreach(var file in Directory.GetFiles(LogsPath))
                {
                    list.Add(Path.GetFileNameWithoutExtension(file));
                }
                return list;
            }
        }

        public static string GetLog(string logName) => File.ReadAllText(Path.Combine(LogsPath, $"{logName}.txt"));
    }
}
