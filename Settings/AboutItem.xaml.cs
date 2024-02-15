using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class AboutItem : Page
    {
        public string appVersion = Package.Current.Id.Version.ToFormattedString();

        public AboutItem()
        {
            this.InitializeComponent();
            edgeVersionCard.Description = $"�汾��{WebViewPage.chromiumVersion}";
            appVersionCard.Description = $"�汾��{appVersion}";
            this.Loaded += CheckAppVersion;
        }

        private async void CheckAppVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.LatestVersion == null)
                {
                    Octokit.GitHubClient client = new(new Octokit.ProductHeaderValue("Edge"));
                    IReadOnlyList<Octokit.RepositoryTag> tags = await client.Repository.GetAllTags("wtcpython", "WinUIEdge");
                    App.LatestVersion = tags[0].Name[1..];
                }
                if (App.LatestVersion.CompareTo(appVersion) > 0)
                {
                    var builder = new AppNotificationBuilder()
                        .AddText($"�����°汾��{App.LatestVersion}���Ƿ�Ҫ���£�\n��ǰ�汾��{appVersion}")
                        .AddArgument("UpdateAppRequest", "ReleaseWebsitePage")
                        .AddButton(new AppNotificationButton("ȷ��")
                            .AddArgument("UpdateAppRequest", "DownloadApp"))
                        .AddButton(new AppNotificationButton("ȡ��"));

                    var notificationManager = AppNotificationManager.Default;
                    notificationManager.Show(builder.BuildNotification());
                }
            }
            catch (Octokit.RateLimitExceededException) { }
        }

        private void CopyText(string text)
        {
            DataPackage package = new()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            package.SetText(text);
            Clipboard.SetContent(package);
        }

        private void TryCopyChromiumVersion(object sender, RoutedEventArgs e)
        {
            CopyText(WebViewPage.chromiumVersion);
        }

        private void TryCopyAppVersion(object sender, RoutedEventArgs e)
        {
            CopyText(appVersion);
        }

        private async void OpenMSEdgeWebsite(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://microsoft.com/zh-cn/edge"));
        }

        private async void OpenRepoWebsite(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/wtcpython/WinUIEdge"));
        }
    }
}
 