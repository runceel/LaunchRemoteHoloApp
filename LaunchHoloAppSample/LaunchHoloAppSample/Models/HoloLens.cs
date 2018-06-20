using Microsoft.Tools.WindowsDevicePortal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Tools.WindowsDevicePortal.DevicePortal;

namespace LaunchHoloAppSample.Models
{
    public class HoloLens : BindableBase
    {
        private static readonly string[] DoNotCloseApps = new string[]
        {
            "HoloShellApp.exe",
            "MixedRealityPortal.exe"
        };

        private DevicePortal _devicePortal;

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private bool _isConnected;
        [JsonIgnore]
        public bool IsConnected
        {
            get { return _isConnected; }
            private set { SetProperty(ref _isConnected, value); }
        }

        private string _message;
        [JsonIgnore]
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        [JsonIgnore]
        public ObservableCollection<HoloLensApplication> Applications { get; } = new ObservableCollection<HoloLensApplication>();

        [JsonIgnore]
        public ObservableCollection<HoloLensProcessInfo> RunningProcesses { get; } = new ObservableCollection<HoloLensProcessInfo>();

        public async Task ConnectAsync()
        {
            if (_devicePortal != null)
            {
                _devicePortal.ConnectionStatus -= DevicePortal_ConnectionStatus;
            }

            // 入力された情報をもとに接続
            _devicePortal = new DevicePortal(new DefaultDevicePortalConnection(Address, UserName, Password));
            _devicePortal.ConnectionStatus += DevicePortal_ConnectionStatus;
            try
            {
                // 証明書をデバイスポータルから取得して、それを使って接続を行う
                await _devicePortal.ConnectAsync(manualCertificate: await _devicePortal.GetRootDeviceCertificateAsync(true));
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Debug.WriteLine(ex);
            }
        }

        public async Task UpdateApplicationsAsync()
        {
            if (_devicePortal == null || !IsConnected)
            {
                Message = "Update app list failed.";
                return;
            }

            Applications.Clear();
            // インストールされているアプリを取得
            var appPackages = await _devicePortal.GetInstalledAppPackagesAsync();
            var apps = appPackages.Packages.Select(x => new HoloLensApplication(x.AppId, x.Name, x.FullName));

            foreach (var app in apps)
            {
                Applications.Add(app);
            }
        }

        public async Task LaunchAppAsync(HoloLensApplication app)
        {
            if (_devicePortal == null || !IsConnected)
            {
                Message = "Launch app failed.";
                return;
            }

            // 実行中のプロセスのリストを取得
            var processes = await _devicePortal.GetRunningProcessesAsync();
            // ターゲットのアプリを起動
            await _devicePortal.LaunchApplicationAsync(app.AppId, app.PackageName);

            var targetProcesses = processes.Processes
                // 間違って自分自身を殺さないようにフィルタ
                .Where(x => x.PackageFullName != app.PackageName)
                // 終了してはいけないプロセスを除外
                .Where(x => !DoNotCloseApps.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                // 空のものは除外
                .Where(x => !string.IsNullOrWhiteSpace(x.PackageFullName));

            // 順次他のプロセスを終了していく
            foreach (var process in targetProcesses)
            {
                try
                {
                    await _devicePortal.TerminateApplicationAsync(process.PackageFullName);
                }
                catch (Exception ex)
                {
                    Message = $"{process.AppName} の停止に失敗しました。";
                    Debug.WriteLine(ex);
                }
            }
        }

        public async Task UpdateRunningProcessesAsync()
        {
            try
            {
                var processes = await _devicePortal.GetRunningProcessesAsync();
                RunningProcesses.Clear();
                foreach (var p in processes.Processes.Where(x => !string.IsNullOrEmpty(x.AppName)).Select(x => new HoloLensProcessInfo(x)))
                {
                    RunningProcesses.Add(p);
                }
            }
            catch (Exception ex)
            {
                Message = $"{ex.Message}";
                Debug.WriteLine(ex);
            }
        }

        private void DevicePortal_ConnectionStatus(DevicePortal sender, DeviceConnectionStatusEventArgs args)
        {
            IsConnected = args.Status == DeviceConnectionStatus.Connected;
            Message = sender.ConnectionFailedDescription;
        }

    }
}
