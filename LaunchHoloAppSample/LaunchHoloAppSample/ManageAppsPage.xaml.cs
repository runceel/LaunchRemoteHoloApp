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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LaunchHoloAppSample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ManageAppsPage : Page
    {
        private HoloLensManager HoloLensManager { get; } = HoloLensManager.Instance;

        public ManageAppsPage()
        {
            this.InitializeComponent();
        }

        private async void AppBarButtonRefreshAppList_Click(object sender, RoutedEventArgs e)
        {
            await UpdateAllAppsAsync();
        }

        private async Task UpdateAllAppsAsync()
        {
            try
            {
                progressRingHost.Visibility = Visibility.Visible;
                await HoloLensManager.Instance.UpdateAllHoloLensApplicationsAsync();
            }
            finally
            {
                progressRingHost.Visibility = Visibility.Collapsed;
            }
        }

        private async void ButtonLaunchApp_Click(object sender, RoutedEventArgs e)
        {
            var app = (HoloLensApplication)((FrameworkElement)sender).DataContext;
            try
            {
                progressRingHost.Visibility = Visibility.Visible;
                await HoloLensManager.LaunchAppAsync(app);
            }
            finally
            {
                progressRingHost.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAllAppsAsync();
        }
    }
}
