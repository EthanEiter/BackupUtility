using System.Diagnostics;
using System.IO;

namespace Backup.Utility.Core
{
    public class CommandRunner
    {
        public static void BackupDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var drive_Name = driveName.Replace(' ', '_');
            var fullLogPath = $"\"{Path.Combine(SettingsManager.LogsPath, $"{drive_Name}_backup_log.txt")}\"";
            BackupRestore(driveRoot, FullBackupPath(rootBackupPath, drive_Name), fullLogPath);
        }

        public static void RestoreDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var drive_Name = driveName.Replace(' ', '_');
            var fullLogPath = $"\"{Path.Combine(SettingsManager.LogsPath, $"{drive_Name}_restore_log.txt")}\"";
            BackupRestore(FullBackupPath(rootBackupPath, drive_Name), driveRoot, fullLogPath);
        }

        private static void BackupRestore(string source, string destination, string logPath)
        {
            CreateLogsDir();
            var close = SettingsManager.CloseConsole ? "/c" : "/k";
            var command = $@"{close} ROBOCOPY {source} {destination} /R:0 /W:0 /mir /V /tee /log:{logPath} && {close} ATTRIB -h -s {destination}";
            Run(command);
        }

        private static string FullBackupPath(string root, string name) => $"\"{Path.Combine(root, name)}\"";
        private static void Run(string command) => Process.Start("CMD.exe", command);
        private static void CreateLogsDir()
        {
            if (!Directory.Exists(SettingsManager.LogsPath))
                Directory.CreateDirectory(SettingsManager.LogsPath);
        }
    }
}
