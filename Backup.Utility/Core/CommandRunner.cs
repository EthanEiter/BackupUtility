using System.Diagnostics;
using System.IO;

namespace Backup.Utility.Core
{
    public class CommandRunner
    {
        public static Process Clone(string driveSourceName, string driveSource, string driveDestination)
        {
            var fullLogPath = $"{Path.Combine(SettingsManager.LogsPath, $"{driveSourceName}_clone_log.txt")}";
            return BackupRestore(driveSource, driveDestination, fullLogPath);
        }

        public static Process BackupDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var drive_Name = string.IsNullOrEmpty(driveName) ? "UNKNOWN" : driveName.Replace(' ', '_');
            var fullLogPath = $"{Path.Combine(SettingsManager.LogsPath, $"{drive_Name}_backup_log.txt")}";
            return BackupRestore(driveRoot, Path.Combine(rootBackupPath, drive_Name), fullLogPath);
        }

        public static Process RestoreDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var drive_Name = string.IsNullOrEmpty(driveName) ? "UNKNOWN" : driveName.Replace(' ', '_');
            var fullLogPath = $"{Path.Combine(SettingsManager.LogsPath, $"{drive_Name}_restore_log.txt")}";
            return BackupRestore(Path.Combine(rootBackupPath, drive_Name), driveRoot, fullLogPath);
        }

        private static Process BackupRestore(string source, string destination, string logPath)
        {
            CreateLogsDir();
            var exit = SettingsManager.CloseConsole ? string.Empty : " -noexit ";
            var command = $" -noprofile {exit}" +
                $"robocopy '{source}' '{destination}' @('/R:0\', '/W:0', '/mir', '/V', '/tee', '/log:{logPath}');" +
                $"attrib -h -s '{destination}'";
            return Process.Start("powershell.exe", command);
        }

        private static void CreateLogsDir()
        {
            if (!Directory.Exists(SettingsManager.LogsPath))
                Directory.CreateDirectory(SettingsManager.LogsPath);
        }
    }
}
