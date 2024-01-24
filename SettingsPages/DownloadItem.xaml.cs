using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;

namespace Edge
{
    public sealed partial class DownloadItem : Page
    {
        public DownloadItem()
        {
            this.InitializeComponent();

            if (Utils.data.DefaultDownloadFolder == string.Empty)
            {
                DownloadFolderCard.Description = GetMoreSpecialFolder.GetSpecialFolder(GetMoreSpecialFolder.SpecialFolder.Downloads);
            }
            else
            {
                DownloadFolderCard.Description = Utils.data.DefaultDownloadFolder;
            }

            setDownloadBehavior.IsOn = Utils.data.AskDownloadBehavior;
            setDownloadFlyout.IsOn = Utils.data.ShowFlyoutWhenStartDownloading;
        }

        private void DownloadBehaviorChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Utils.data.AskDownloadBehavior = setDownloadBehavior.IsOn;
        }

        private void ShowFlyoutChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Utils.data.ShowFlyoutWhenStartDownloading = setDownloadFlyout.IsOn;
        }

        private async void ChangeDownloadFolder(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FolderPicker picker = new();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                DownloadFolderCard.Description = Utils.data.DefaultDownloadFolder = folder.Name;
            }
        }
    }
}