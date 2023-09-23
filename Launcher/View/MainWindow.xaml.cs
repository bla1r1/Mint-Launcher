//using
#region
using DiscordRPC;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Button = DiscordRPC.Button;
using System.Threading.Tasks;
using System.IO.Compression;
#endregion

namespace Minty
{
    public partial class MainWindow : Window
    {
        private TaskbarIcon taskbarIcon;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();
            VerCheckLauncher();
            DiscordRPC();
        }
        //metods
        #region
        //VERCHECK
        #region
        private async void VerCheckLauncher()
            {
                string updateFilePath = "LauncherUpdater.exe";
                string versionUrllauncher = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
                string versionText = await DownloadVersionText(versionUrllauncher);


                if (versionText != null)
                {
                    double latestVersion = .0;

                    if (!Double.TryParse(versionText, out latestVersion))
                    {
                        MessageBox.Show("Unable to parse: " + versionText);
                        return;
                    }

                    double currentVersion = 1.15;

                    if (currentVersion < latestVersion)
                    {
                        LaunchExecutable(updateFilePath);
                        Environment.Exit(0);
                    }

                }
            }
            public async Task<string> DownloadVersionText(string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        System.Windows.MessageBox.Show("Unable to connect to the web server.");
                        return null;
                    }

                    string versionText = await response.Content.ReadAsStringAsync();
                    return versionText;
                }
            }
            private void LaunchExecutable(string exePath)
            {
                try
                {
                    Process.Start(exePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error launching executable: {ex.Message}");
                }
            }
        #endregion
        //taskbar
        #region
        private void InitializeTrayIcon()
        {
            taskbarIcon = new TaskbarIcon();
            taskbarIcon.Icon = new System.Drawing.Icon("L_images/virus.ico");
            taskbarIcon.ToolTipText = "Minty";

            ContextMenu contextMenu = new ContextMenu();

            MenuItem openMenuItem = new MenuItem() { Header = "Open" };
            openMenuItem.Click += OpenMenuItem_Click;
            contextMenu.Items.Add(openMenuItem);

            MenuItem exitMenuItem = new MenuItem() { Header = "Exit" };
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            MenuItem minimizeToTrayMenuItem = new MenuItem() { Header = "Minimize to Tray" };
            minimizeToTrayMenuItem.Click += MinimizeToTrayMenuItem_Click;
            contextMenu.Items.Add(minimizeToTrayMenuItem);

            taskbarIcon.ContextMenu = contextMenu;

            taskbarIcon.TrayLeftMouseDown += TaskbarIcon_LeftMouseDown;

            taskbarIcon.Visibility = Visibility.Visible;
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            taskbarIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        public void MinimizeToTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TaskbarIcon_LeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            taskbarIcon.Dispose();
            base.OnClosed(e);
        }
        public void MinimizeToTray()
        {
            this.Hide();
        }
        private async Task Ico()
        {
            string IcoFilePath = "L_images/virus.ico";
            string FilePath = "L_images";
            string downloadUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/virus.ico";

            Directory.CreateDirectory(FilePath);
            try
            {
                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), IcoFilePath);
                }
            }

            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
            Application.Current.Shutdown();
        }
        #endregion
        //Rcp
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1155073793224609843");

        public static void InitRPC()
        {
            client.OnReady += (sender, e) => { };

            client.OnPresenceUpdate += (sender, e) => { };

            client.OnError += (sender, e) => { };

            client.Initialize();
        }
        public static void UpdateRPC()
        {
            var presence = new RichPresence()
            {
                State = "Minty",
                Details = "Hacking MHY <333",

                Assets = new Assets()
                {
                    LargeImageKey = "virus",
                    SmallImageKey = "",
                    SmallImageText = "Minty Launcher"
                },
                Buttons = new Button[]
                {
                    new Button()
                    {
                        Label = "Join",
                        Url = "https://discord.gg/kindawindytoday"
                    }
                }
            };
            client.SetPresence(presence);
            client.Invoke();
        }



        public static void DiscordRPC()
        {

            if (!client.IsInitialized)
            {
                InitRPC();
            }

            UpdateRPC();
        }
        #endregion
        //dragmove
        #region
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
        //Buttons
        #region  
        private void Button_Click_Discord(object sender, RoutedEventArgs e)
        {
            string link = "https://discord.gg/CpGbZSHKcD";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });
        }

        private void Button_Click_Git(object sender, RoutedEventArgs e)
        {

            string link = "https://github.com/kindawindytoday/Minty-Old";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Boosty(object sender, RoutedEventArgs e)
        {

            string link = "https://boosty.to/kindawindytoday";


            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Tiktok(object sender, RoutedEventArgs e)
        {

            string link = "https://www.tiktok.com/@kindawindytoday?ug_source=op.auth&ug_term=Linktr.ee&utm_source=awyc6vc625ejxp86&utm_campaign=tt4d_profile_link&_r=1";

            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Youtube(object sender, RoutedEventArgs e)
        {

            string link = "https://www.youtube.com/@KindaWindyToday";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }

        #endregion
        #endregion
    }
}
