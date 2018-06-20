using LaunchHoloAppSample.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LaunchHoloAppSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RunningProcessesInfoPage : Page
    {
        private readonly Subject<Unit> _filterUpdateTrigger = new Subject<Unit>();
        private HoloLensManager HoloLensManager { get; } = HoloLensManager.Instance;

        public RunningProcessesInfoPage()
        {
            this.InitializeComponent();
            _filterUpdateTrigger.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ =>
                {
                    HoloLensManager.RunningProcessNameFilter = textBoxFilter.Text;
                });
        }

        private async void AppBarButtonRefreshRunningProcesses_Click(object sender, RoutedEventArgs e) => await UpdateRunningProcessesInfoAsync();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateRunningProcessesInfoAsync();
        }

        private Task UpdateRunningProcessesInfoAsync() => HoloLensManager.UpdateRunningProcessesInfoAsync();

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterUpdateTrigger.OnNext(Unit.Default);
        }
    }
}
