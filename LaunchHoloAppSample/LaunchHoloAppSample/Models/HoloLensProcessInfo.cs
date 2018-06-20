using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Tools.WindowsDevicePortal.DevicePortal;

namespace LaunchHoloAppSample.Models
{
    public class HoloLensProcessInfo
    {
        private readonly DeviceProcessInfo _deviceProcessInfo;

        public string Name => $"{_deviceProcessInfo.AppName}({_deviceProcessInfo.ProcessId})";

        public HoloLensProcessInfo(DeviceProcessInfo deviceProcessInfo)
        {
            _deviceProcessInfo = deviceProcessInfo;
        }
    }
}
