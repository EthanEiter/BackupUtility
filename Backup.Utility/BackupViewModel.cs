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
        public void Exit() => Process.GetCurrentProcess().CloseMainWindow();


        public bool PathVis
        {
            get { return SettingsManager.PathVisibility; }
            set
            {
                SettingsManager.PathVisibility = value;
                NotifyOfPropertyChange(() => PathVisibility);
                NotifyOfPropertyChange(() => PathVis);
            }
        }
        public Visibility PathVisibility => PathVis ? Visibility.Visible : Visibility.Collapsed;


        public bool LogVis
        {
            get { return SettingsManager.LogVisibility; }
            set
            {
                SettingsManager.LogVisibility = value;
                NotifyOfPropertyChange(() => LogVisibility);
                NotifyOfPropertyChange(() => LogVis);
            }
        }
        public Visibility LogVisibility => LogVis ? Visibility.Visible : Visibility.Collapsed;


        public bool CloseConsole
        {
            get { return SettingsManager.CloseConsole; }
            set
            {
                SettingsManager.CloseConsole = value;
                NotifyOfPropertyChange(() => CloseConsole);
            }
        }
            
        
        public string Location =>  "< SELECT BACKUP PATH >";


        public string BackupPath
        {
            get{ return SettingsManager.BackupPath; }
            set
            {
                SettingsManager.BackupPath = value;
                NotifyOfPropertyChange(() => BackupPath);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }


        public void Browse()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            BackupPath = dialog.SelectedPath;
        }


        public string Instruction => SettingsManager.DrivesVisibility ? "< SELECT DRIVE >" : "< SELECT PATH TO BACKUP >";


        public bool DriveVis
        {
            get { return SettingsManager.DrivesVisibility; }
            set
            {
                SettingsManager.DrivesVisibility = value;
                NotifyOfPropertyChange(() => DriveVisibility);
                NotifyOfPropertyChange(() => DriveVis);
                NotifyOfPropertyChange(() => BackupSourceVisibility);
                NotifyOfPropertyChange(() => Instruction);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }
        public Visibility DriveVisibility => DriveVis ? Visibility.Visible : Visibility.Collapsed;
        public Visibility BackupSourceVisibility => DriveVis ? Visibility.Collapsed : Visibility.Visible;


        private string _drive;
        public string SelectedDrive
        {
            get { return _drive; }
            set
            {
                _drive = value;
                NotifyOfPropertyChange(() => SelectedDrive);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }
        public IEnumerable<string> Drives => DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable || x.DriveType == DriveType.Fixed).Select(x => $"({x.RootDirectory}) {x.VolumeLabel}");


        public void RefreshDrives() => NotifyOfPropertyChange(() => Drives);


        public string BackupSource
        {
            get { return SettingsManager.BackupSource; }
            set
            {
                SettingsManager.BackupSource = value;
                NotifyOfPropertyChange(() => BackupSource);
                NotifyOfPropertyChange(() => CanBackup);
                NotifyOfPropertyChange(() => CanRestore);
            }
        }

        public void BrowseSource()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            BackupSource = dialog.SelectedPath;
        }


        private DriveInfo GetSelectedDrive => DriveInfo.GetDrives().First(x => SelectedDrive.Contains(x.RootDirectory.ToString()) && SelectedDrive.Contains(x.VolumeLabel));
        public bool CanBackup
        {
            get
            {
                if (DriveVis)
                    return !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(SelectedDrive));
                return !(string.IsNullOrEmpty(BackupPath) || string.IsNullOrEmpty(BackupSource));
            }
        }
        public void Backup()
        {
            if (SettingsManager.DrivesVisibility)
            {
                Output = $"BACKING UP {SelectedDrive}";

                var drive = GetSelectedDrive;
                CommandRunner.BackupDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
            }
            else
            {
                var Name = Path.GetFileName(BackupSource);

                Output = $"BACKING UP {Name}";

                CommandRunner.BackupDrive(Name, BackupSource, BackupPath);
            }
        }


        public bool CanRestore => CanBackup;
        public void Restore()
        {
            if (SettingsManager.DrivesVisibility)
            {
                Output = $"RESTORING {SelectedDrive}";

                var drive = GetSelectedDrive;
                CommandRunner.RestoreDrive(drive.VolumeLabel, drive.RootDirectory.ToString(), BackupPath);
            }
            else
            {
                var Name = Path.GetFileName(BackupSource);

                Output = $"RESTORING {Name}";

                CommandRunner.RestoreDrive(Name, BackupSource, BackupPath);
            }
        }


        private string _output = "OUTPUT> _";
        public string Output
        {
            get { return _output; }
            set
            {
                _output = $"OUTPUT: {value}_";
                NotifyOfPropertyChange(() => Output);
            }
        }


        public string LogLabel => "< VIEW LOGS >";


        public List<string> Logs => SettingsManager.LogList;
        private string _log;
        public string SelectedLog
        {
            get { return _log; }
            set
            {
                _log = value;
                NotifyOfPropertyChange(() => SelectedLog);
                Viewer = SettingsManager.GetLog(SelectedLog);
            }
        }


        public void RefreshLogs() => NotifyOfPropertyChange(() => Logs);


        private string _viewer;
        public string Viewer
        {
            get { return _viewer; }
            set
            {
                _viewer = value;
                NotifyOfPropertyChange(() => Viewer);
            }
        }
    }
}