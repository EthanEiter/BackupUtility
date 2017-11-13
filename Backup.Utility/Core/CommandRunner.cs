using System.Diagnostics;
using System.IO;

namespace Backup.Utility.Core
{
    public class CommandRunner
    {
        public static Process Clone(string driveSourceName, string driveSource, string driveDestination) =>
            Start(driveSource, driveDestination, LogManager.GetFullLogPath(driveSourceName, Functions.Clone));

        public static Process BackupDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var name = LogManager.NameFixer(driveName);
            return Start(driveRoot, Path.Combine(rootBackupPath, name),
                LogManager.GetFullLogPath(name, Functions.Backup));
        }

        public static Process RestoreDrive(string driveName, string driveRoot, string rootBackupPath)
        {
            var name = LogManager.NameFixer(driveName);
            return Start(Path.Combine(rootBackupPath, name), driveRoot,
                LogManager.GetFullLogPath(name, Functions.Restore));
        }

        private static Process Start(string source, string destination, string logPath)
        {
            var exit = SettingsManager.CloseConsole ? string.Empty : " -noexit ";
            var command = $" -noprofile {exit}" +
                $"robocopy '{source}' '{destination}' @('/R:0\', '/W:0', '/mir', '/V', '/tee', '/log:{logPath}');" +
                $"attrib -h -s '{destination}'";
            return Process.Start("powershell.exe", command);
        }
    }
}
