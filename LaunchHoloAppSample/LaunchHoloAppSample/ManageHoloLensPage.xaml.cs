using LaunchHoloAppSample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class ManageHoloLensPage : Page
    {
        private HoloLensManager HoloLensManager { get; } = HoloLensManager.Instance;

        public ManageHoloLensPage()
        {
            this.InitializeComponent();
        }

        private async void AddHoloLensButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxAddress.Text) ||
                string.IsNullOrWhiteSpace(textBoxUserName.Text) ||
                string.IsNullOrWhiteSpace(passwordBoxPassword.Password))
            {
                var dialog = new MessageDialog("未入力の項目があります");
                await dialog.ShowAsync();
                return;
            }

            HoloLensManager.AddManagedHoloLens(new HoloLens
            {
                Name = textBoxName.Text,
                Address = $"https://{textBoxAddress.Text}",
                UserName = textBoxUserName.Text,
                Password = passwordBoxPassword.Password,
            });

            textBoxName.Text = "";
            textBoxAddress.Text = "";
            textBoxUserName.Text = "";
            passwordBoxPassword.Password = "";

            ApplicationData.Current.LocalSettings.Values["data"] = HoloLensManager.Instance.SerializeToJson();
        }

        private async void RefreshConnectionStatusButton_Click(object sender, RoutedEventArgs e)
        {
            await HoloLensManager.Instance.ConnectAllAsync();
        }

        private void SwipeItemDeleteHoloLens_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            var target = args.SwipeControl.DataContext as HoloLens;
            HoloLensManager.Instance.RemoveManagedHoloLens(target);

            ApplicationData.Current.LocalSettings.Values["data"] = HoloLensManager.Instance.SerializeToJson();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await HoloLensManager.Instance.ConnectAllAsync();
        }
    }
}
