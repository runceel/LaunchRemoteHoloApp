using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LaunchHoloAppSample
{
    static class DispatcherHelper
    {
        public static CoreDispatcher Dispatcher { get; set; }

        public static Task DispatchAsync(Action action)
        {
            if (Dispatcher == null)
            {
                action();
                return Task.CompletedTask;
            }

            if (Dispatcher.HasThreadAccess)
            {
                action();
                return Task.CompletedTask;
            }

            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask();
        }
    }
}
