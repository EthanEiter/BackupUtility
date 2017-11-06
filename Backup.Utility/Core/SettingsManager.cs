using Backup.Utility.Properties;

namespace Backup.Utility.Core
{
    public static class SettingsManager
    {
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
    }
}
