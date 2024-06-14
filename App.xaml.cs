using Edge.Data;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.Storage;


namespace Edge
{
    public partial class App : Application
    {
        public static List<MainWindow> mainWindows = [];
        public static List<TabViewItem> ClosedTabs = [];
        public static string LatestVersion = null;
        public static Dictionary<string, JsonElement> settings;
        public static WebView2 webView2 = new();

        public App()
        {
            this.InitializeComponent();
            EnsureWebView2Async();
        }

        public async void EnsureWebView2Async()
        {
            await webView2.EnsureCoreWebView2Async();
        }

        public static MainWindow CreateNewWindow()
        {
            MainWindow window = new();
            window.Closed += (sender, e) =>
            {
                mainWindows.Remove(window);
                File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "/settings.json", JsonSerializer.Serialize(settings));
            };
            mainWindows.Add(window);
            return window;
        }

        public static MainWindow GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (MainWindow window in mainWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        public static JsonElement ToJsonElement<T>(T value)
        {
            return JsonDocument.Parse(JsonSerializer.Serialize(value)).RootElement;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            string path = Info.CheckUserSettingData();
            settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path));

            m_window = CreateNewWindow();
            m_window.Activate();

            AppNotificationManager notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
            notificationManager.Register();

            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var activationKind = activatedArgs.Kind;
            if (activationKind != ExtendedActivationKind.AppNotification)
            {
                OverlappedPresenter presenter = m_window.AppWindow.Presenter as OverlappedPresenter;
                presenter.Restore(true);
            }
            else
            {
                HandleNotification((AppNotificationActivatedEventArgs)activatedArgs.Data);
            }
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            var dispatcherQueue = m_window?.DispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            dispatcherQueue.TryEnqueue(delegate
            {
                switch (args.Arguments["UpdateAppRequest"])
                {
                    case "ReleaseWebsitePage":
                        mainWindows[0].AddNewTab(new WebViewPage() { WebUri = "https://github.com/wtcpython/WinUIEdge/releases/latest/" });
                        break;
                    case "DownloadApp":
                        mainWindows[0].AddNewTab(new WebViewPage() { WebUri = $"https://github.com/wtcpython/WinUIEdge/releases/latest/download/Edge_{LatestVersion}_x64.7z" });
                        break;
                }
            });
        }

        private void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleNotification(args);
        }

        private static Window m_window;
    }
}
