using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchHoloAppSample.Models
{
    public class HoloLensApplication
    {
        public string AppId { get; }
        public string Name { get; }
        public string PackageName { get; }

        public HoloLensApplication(string appId, string name, string packageName)
        {
            AppId = appId;
            Name = name;
            PackageName = packageName;
        }

        public static IEqualityComparer<HoloLensApplication> DefaultComparer { get; } = new Comparer();

        class Comparer : IEqualityComparer<HoloLensApplication>
        {
            public bool Equals(HoloLensApplication x, HoloLensApplication y)
            {
                return x?.AppId == y?.AppId;
            }

            public int GetHashCode(HoloLensApplication obj)
            {
                return obj.AppId.GetHashCode();
            }
        }
    }
}
