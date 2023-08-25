//using
#region
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using WpfApp1.View;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Forms;
using DiscordRPC;
using Button = DiscordRPC.Button;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Threading;
using System.Linq;
#endregion
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static Mutex mutex = new Mutex(true, "{FE871BB4-5D95-41B1-ADFD-B95113C0B1D3}");
        private TaskbarIcon taskbarIcon;
        public MainWindow()
        {
            InitializeComponent();
            //mutexapp();
            checkversion();
            InitializeTrayIcon();
            DiscordRPC();
            video();
        }
        //metods
        #region
        //checkver
        #region
        public async void checkversion()
        {
            string versionUrl = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty .zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");
            string updateUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/update.exe";
            string tempFolderPath = System.IO.Path.GetTempPath();
            string updateFilePath = System.IO.Path.Combine(tempFolderPath, "update.exe");
            try
            {
                string versionText = await DownloadVersionText(versionUrl);

                if (versionText != null)
                {
                    double latestVersion = .0;

                    if (!Double.TryParse(versionText, out latestVersion))
                    {
                        System.Windows.MessageBox.Show("Unable to parse: " + versionText);
                        return;
                    }

                    double currentVersion = 1.11;

                    if (currentVersion < latestVersion)
                    {
                        File.Delete(verfilePath);
                        File.Delete(launcherFilePath);
                        File.Delete(dllFilePath);

                        await DownloadFile(updateUrl, updateFilePath);
                        await Task.Delay(2000);
                        LaunchExecutable(updateFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error retrieving launcher version: {ex.Message}");
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
        #endregion
        //taskbar
        #region

        private void InitializeTrayIcon()
        {
            taskbarIcon = new TaskbarIcon();
            taskbarIcon.Icon = new System.Drawing.Icon("icon.ico");
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
        #endregion
        //video
        #region
        public async void video()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "log");
            string vidFilePath = System.IO.Path.Combine(assetsFolderPath, "video.mp4");
            string logfilePath = System.IO.Path.Combine(assetsFolderPath, "log.txt");
            string logtext = "1";
            string vidUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/video.mp4";
            Directory.CreateDirectory(mintyFolderPath);
            Directory.CreateDirectory(assetsFolderPath);
            using (File.Create(logfilePath)) ;
            if (!File.Exists(vidFilePath))
            {
                await DownloadFile(vidUrl, vidFilePath);
                string verfileContent = File.ReadAllText(logfilePath);
                if (verfileContent.Contains(logtext))
                {

                }
                else
                {
                    VideoWindow videoWindow = new VideoWindow();
                    videoWindow.Show();
                }
            }

        }
        #endregion
        //download
        #region
        public async Task DownloadFile(string url, string destinationPath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                }
            }
            catch (HttpRequestException ex)
            {
                System.Windows.MessageBox.Show($"Error downloading file: {ex.Message}");
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show($"Error saving file: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion
        //launch
        #region
        public void LaunchExecutable(string exePath)
        {
            try
            {
                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error launching executable: {ex.Message}");
            }
        }
        #endregion
        //RPC
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1112360491847778344");

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
                    LargeImageKey = "idol",
                    SmallImageKey = "gensh",
                    SmallImageText = "Genshin Impact"
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
            InitRPC();
            UpdateRPC();
        }


        #endregion
        //if app started
        #region
        public static void mutexapp()
        {
            App app = new App();

            {
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    try
                    {
                        
                        app.Run(new MainWindow());
                    }
                   finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Another instance is already running.", "App is already running", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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
