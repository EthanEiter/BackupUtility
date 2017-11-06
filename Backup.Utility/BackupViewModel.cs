using System.Collections.Generic;
using Caliburn.Micro;
using System.Diagnostics;
using System.Windows.Forms;
using Backup.Utility.Core;
using System.IO;
using System.Linq;

namespace Backup.Utility
{
    public class BackupViewModel : PropertyChangedBase
    {
        private string _viewer;
        private string _drive;
        private string _driveSource;
        private string _driveDest;
        private string _log;
        private string _output;

        public bool PathVisibility
        {
            get => SettingsManager.PathVisibility;
            set
            {
                SettingsManager.PathVisibility = value;
                NotifyOfPropertyChange(() => PathVisibility);
            }
        }

        public bool LogVisibility
        {
            get => SettingsManager.LogVisibility;
            set
            {
                SettingsManager.LogVisibility = value;
                NotifyOfPropertyChange(() => LogVisibility);
            }
        }

        public bool CloseConsole
        {
            get => SettingsManager.CloseConsole;
            set
            {
                SettingsManager.CloseConsole = value;
                NotifyOfPropertyChange(() => CloseConsole);
            }
        }

        public bool CloneDrive
        {
            get => SettingsManager.Clone;
            set
            {
                SettingsManager.Clone = value;
                NotifyOfPropertyChange(() => CloneDrive);
            }
        }

        public string BackupPath
        {
            get => SettingsManager.BackupPath;
            set
            {
                SettingsManager.BackupPath = value;
                NotifyOfPropertyChange(() => BackupPath);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }

        public bool DriveVisibility
        {
            get => SettingsManager.DrivesVisibility;
            set
            {
                SettingsManager.DrivesVisibility = value;
                NotifyOfPropertyChange(() => DriveVisibility);
                NotifyOfPropertyChange(() => Instruction);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }

        public string SelectedDrive
        {
            get => _drive;
            set
            {
                _drive = value;
                NotifyOfPropertyChange(() => SelectedDrive);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }

        public string SelectedDrivesSource
        {
            get => _driveSource;
            set
            {
                _driveSource = value;
                NotifyOfPropertyChange(() => DrivesDestination);
                NotifyOfPropertyChange(() => CanClone);
            }
        }

        public string SelectedDrivesDestination
        {
            get => _driveDest;
            set
            {
                _driveDest = value;
                NotifyOfPropertyChange(() => DrivesSource);
                NotifyOfPropertyChange(() => CanClone);
            }
        }

        public string BackupSource
        {
            get => SettingsManager.BackupSource;
            set
            {
                SettingsManager.BackupSource = value;
                NotifyOfPropertyChange(() => BackupSource);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }

        public string SelectedLog
        {
            get => _log;
            set
            {
                _log = value;
                NotifyOfPropertyChange(() => SelectedLog);
                Viewer = Core.LogManager.GetLog(SelectedLog);
            }
        }

        public string Viewer
        {
            get => _viewer;
            set
            {
                _viewer = value;
                NotifyOfPropertyChange(() => Viewer);
            }
        }

        public string Output
        {
            get => $"OUTPUT> {_output}";
            set
            {
                _output = value;
                NotifyOfPropertyChange(() => Output);
            }
        }

        public void Browse()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            BackupPath = dialog.SelectedPath;
        }

        public bool CanRestore => CanBackup;

        public IEnumerable<string> Drives 
            => DriveInfo.GetDrives()
                .Where(x => x.DriveType == DriveType.Removable || x.DriveType == DriveType.Fixed)
                .Select(x => $"({x.RootDirectory}) {x.VolumeLabel}");


        public void RefreshDrives() => NotifyOfPropertyChange(() => Drives);

        public IEnumerable<string> DrivesSource => Drives.Where(x => !x.Equals(SelectedDrivesDestination));

        public IEnumerable<string> DrivesDestination => Drives.Where(x => !x.Equals(SelectedDrivesSource));

        public void RefreshDrivesSource()
        {
            NotifyOfPropertyChange(() => Drives);
            NotifyOfPropertyChange(() => DrivesSource);
            NotifyOfPropertyChange(() => DrivesDestination);
        }

        public void BrowseSource()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            BackupSource = dialog.SelectedPath;
        }


        private DriveInfo GetSelectedDrive(string drive)
            => DriveInfo.GetDrives().First(x => drive.Contains(x.RootDirectory.ToString()) && drive.Contains(x.VolumeLabel));
        public bool CanBackup
            => DriveVisibility 
                ? !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(SelectedDrive)) 
                : !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(BackupSource));

        public void Backup()
        {
            if (DriveVisibility)
            {
                Output = $"BACKING UP {SelectedDrive.ToUpper()}";

                var drive = GetSelectedDrive(SelectedDrive);
                var process = CommandRunner.BackupDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{SelectedDrive.ToUpper()} BACKUP COMPLETE";
                    NotifyOfPropertyChange(() => Logs);
                    SelectedLog = Core.LogManager.GetLogName(drive.VolumeLabel, "backup", true);
                };
            }
            else
            {
                var name = Path.GetFileName(BackupSource);
                if (name == null)
                {
                    return;
                }

                Output = $"BACKING UP {name.ToUpper()}";

                var process = CommandRunner.BackupDrive(name, BackupSource, BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{name.ToUpper()} BACKUP COMPLETE";
                    NotifyOfPropertyChange(() => Logs);
                    SelectedLog = Core.LogManager.GetLogName(name, "backup", true);
                };
            }
        }
        public void Restore()
        {
            if (DriveVisibility)
            {
                Output = $"RESTORING {SelectedDrive.ToUpper()}";

                var drive = GetSelectedDrive(SelectedDrive);
                var process = CommandRunner.RestoreDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{SelectedDrive.ToUpper()} RESTORE COMPLETE";
                    NotifyOfPropertyChange(() => Logs);
                    SelectedLog = Core.LogManager.GetLogName(drive.VolumeLabel, "restore", true);
                };
            }
            else
            {
                var name = Path.GetFileName(BackupSource);
                if (name == null)
                {
                    return;
                }

                Output = $"RESTORING {name.ToUpper()}";

                var process = CommandRunner.RestoreDrive(name, BackupSource, BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{name.ToUpper()} RESTORE COMPLETE";
                    NotifyOfPropertyChange(() => Logs);
                    SelectedLog = Core.LogManager.GetLogName(name, "restore", true);
                };
            }
        }

        public void Clone()
        {
            Output = $"CLONING {SelectedDrivesSource.ToUpper()} TO {SelectedDrivesDestination.ToUpper()}";
            var sourceDrive = GetSelectedDrive(SelectedDrivesSource);
            var destDrive = GetSelectedDrive(SelectedDrivesDestination);
            var process = CommandRunner.Clone(sourceDrive.VolumeLabel, sourceDrive.RootDirectory.ToString(), destDrive.RootDirectory.ToString());
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => {
                Output = "CLONE COMPLETE";
                NotifyOfPropertyChange(() => Logs);
                SelectedLog = Core.LogManager.GetLogName(sourceDrive.VolumeLabel, "restore", true);
            };
        }

        public bool CanClone 
            => !string.IsNullOrWhiteSpace(SelectedDrivesSource) 
            && !string.IsNullOrWhiteSpace(SelectedDrivesDestination);

        public List<string> Logs => Core.LogManager.LogList;

        public void RefreshLogs() 
            => NotifyOfPropertyChange(() => Logs);

        public void Exit() => Process.GetCurrentProcess().CloseMainWindow();

        public static string BrowseButtonText => "BROWSE";
        public static string RefreshButtonText => "REFRESH";
        public static string BackupButtonText => "BACKUP";
        public static string RestoreButtonText => "RESTORE";
        public static string CloneButtonText => "CLONE";

        public static string FileMenuItemText => "< FILE >";
        public static string ViewMenuItemText => "< VIEW >";
        public static string OptionsMenuItemText => "< OPTIONS >";

        public static string ExitMenuItemText => "Exit";
        public static string RemovableDrivesMenuItemText => "Removable Drives";
        public static string BackupPathMenuItemText => "Backup Path";
        public static string LogViewerMenuItemText => "Log Viewer";
        public static string CloseConsoleMenuItemText => "Close Console";
        public static string CloneDriveMenuItemText => "Clone Drive";

        public string LogLabel => "< VIEW LOGS >";
        public string Location => "< SELECT BACKUP PATH >";
        public string Instruction => SettingsManager.DrivesVisibility ? "< SELECT DRIVE >" : "< SELECT PATH TO BACKUP >";

        public string DriveCloneSourceLabel => "< SELECT SOURCE DRIVE >";
        public string DriveCloneDestinationLabel => "<SELECT DESTINATION DRIVE >";
    }
}