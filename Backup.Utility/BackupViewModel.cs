using System.Collections.Generic;
using Caliburn.Micro;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows;
using Backup.Utility.Core;
using System.IO;
using System.Linq;

namespace Backup.Utility
{
    public class BackupViewModel : PropertyChangedBase
    {
        private string _viewer;
        private string _drive;
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
                NotifyOfPropertyChange(() => BackupSourceVisibility);
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
                Viewer = SettingsManager.GetLog(SelectedLog);
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
            var result = dialog.ShowDialog();
            BackupPath = dialog.SelectedPath;
        }

        public bool CanRestore => CanBackup;

        public Visibility BackupSourceVisibility => DriveVisibility ? Visibility.Collapsed : Visibility.Visible;

        public IEnumerable<string> Drives 
            => DriveInfo.GetDrives()
                .Where(x => x.DriveType == DriveType.Removable || x.DriveType == DriveType.Fixed)
                .Select(x => $"({x.RootDirectory}) {x.VolumeLabel}");


        public void RefreshDrives() => NotifyOfPropertyChange(() => Drives);

        public void BrowseSource()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            BackupSource = dialog.SelectedPath;
        }


        private DriveInfo GetSelectedDrive => DriveInfo.GetDrives().First(x => SelectedDrive.Contains(x.RootDirectory.ToString()) && SelectedDrive.Contains(x.VolumeLabel));
        public bool CanBackup
            => DriveVisibility 
                ? !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(SelectedDrive)) 
                : !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(BackupSource));

        public void Backup()
        {
            if (DriveVisibility)
            {
                Output = $"BACKING UP {SelectedDrive}";

                var drive = GetSelectedDrive;
                var process = CommandRunner.BackupDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{SelectedDrive} BACKUP COMPLETE";
                };
            }
            else
            {
                var Name = Path.GetFileName(BackupSource);

                Output = $"BACKING UP {Name}";

                var process = CommandRunner.BackupDrive(Name, BackupSource, BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{Name} BACKUP COMPLETE";
                };
            }
        }
        public void Restore()
        {
            if (DriveVisibility)
            {
                Output = $"RESTORING {SelectedDrive}";

                var drive = GetSelectedDrive;
                var process = CommandRunner.RestoreDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{SelectedDrive} RESTORE COMPLETE";
                };
            }
            else
            {
                var Name = Path.GetFileName(BackupSource);

                Output = $"RESTORING {Name}";

                var process = CommandRunner.RestoreDrive(Name, BackupSource, BackupPath);
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => {
                    Output = $"{Name} RESTORE COMPLETE";
                };
            }
        }

        public List<string> Logs => SettingsManager.LogList;

        public void RefreshLogs() 
            => NotifyOfPropertyChange(() => Logs);

        public void Exit() => Process.GetCurrentProcess().CloseMainWindow();

        public static string BrowseButtonText => "BROWSE";
        public static string RefreshButtonText => "REFRESH";
        public static string BackupButtonText => "BACKUP";
        public static string RestoreButtonText => "RESTORE";

        public static string FileMenuItemText => "< FILE >";
        public static string ViewMenuItemText => "< VIEW >";
        public static string OptionsMenuItemText => "< OPTIONS >";

        public static string ExitMenuItemText => "Exit";
        public static string RemovableDrivesMenuItemText => "Removable Drives";
        public static string BackupPathMenuItemText => "Backup Path";
        public static string LogViewerMenuItemText => "Log Viewer";
        public static string CloseConsoleMenuItemText => "Close Console";

        public string LogLabel => "< VIEW LOGS >";
        public string Location => "< SELECT BACKUP PATH >";
        public string Instruction => SettingsManager.DrivesVisibility ? "< SELECT DRIVE >" : "< SELECT PATH TO BACKUP >";
    }
}