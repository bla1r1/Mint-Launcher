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
using DiscordRpcDemo;
using System.Windows.Forms;

namespace WpfApp1
{


    public partial class MainWindow : Window
    {
        private NotifyIcon trayIcon;
        private DiscordRpc.EventHandlers handlers;
        private DiscordRpc.RichPresence presence;

        public MainWindow()
        {
            InitializeComponent();
            video();
            checkversion();
            DiscordRcp();
            trayIcon = new NotifyIcon();
            trayIcon.Icon = new System.Drawing.Icon("icon.ico");
            trayIcon.Text = "Minty"; 
            trayIcon.Visible = true;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
        }
        

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

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
                using (File.Create(logfilePath));
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

                    double currentVersion = 1.9;

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
        public async void DiscordRcp()
        {
            this.handlers = default(DiscordRpc.EventHandlers);
            DiscordRpc.Initialize("1112360491847778344", ref this.handlers, true, null);
            this.handlers = default(DiscordRpc.EventHandlers);
            DiscordRpc.Initialize("1112360491847778344", ref this.handlers, true, null);
            this.presence.details = "Minty";
            this.presence.state = "Hacking MHY <333";
            this.presence.largeImageKey = "idol";
            this.presence.smallImageKey = "gensh";
            this.presence.largeImageText = "Genshin Impact";
            this.presence.smallImageText = "";
            DiscordRpc.UpdatePresence(ref this.presence);

        }
        //metods
        #region
        //dowloadver
        #region
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
        //EXTRACT
        #region
        public async Task ExtractZipFile(string zipFilePath, string extractionPath)
        {
            try
            {
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(zipFilePath, extractionPath);
                });

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error while extracting the archive: " + ex.Message);
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
        //taskbar
        #region
        public void TrayIcon_DoubleClick(object sender, EventArgs e)
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
            trayIcon.Dispose();
            base.OnClosed(e);
        }
        public void MinimizeToTray()
        {
            this.Hide();
        }
        #endregion

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
    }
}
