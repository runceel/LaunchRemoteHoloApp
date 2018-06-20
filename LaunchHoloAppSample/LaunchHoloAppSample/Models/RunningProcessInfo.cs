using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchHoloAppSample.Models
{
    public class RunningProcessInfo : BindableBase
    {
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { SetProperty(ref _deviceName, value); }
        }

        private IEnumerable<HoloLensProcessInfo> _runningProcesses;
        public IEnumerable<HoloLensProcessInfo> RunningProcesses
        {
            get { return _runningProcesses; }
            set { SetProperty(ref _runningProcesses, value); }
        }
    }
}
