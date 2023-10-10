using System;
using System.IO.Compression;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Windows;
using System.Diagnostics;
using Updater.View;


namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Window_Loaded();
        }

        private async void Window_Loaded()
        {
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LauncherFolderPath = System.IO.Path.Combine(MainFolderPath, "Launcher");
            string LauncherFilePath = System.IO.Path.Combine(LauncherFolderPath, "Launcher.exe");
            string VerUrlLauncher = "https://github.com/sad-akulka/LauncherVer/releases/download/1.0/LaunchVer.txt";
            string versionText = await DownloadVersionText(VerUrlLauncher);
            if (!File.Exists(LauncherFilePath))
            {
                Install InstallWIndow = new Install();
                InstallWIndow.Show();
                this.Close();
            }
            else
            {
                if (versionText != null)
                {
                    double latestVersion = .0;

                    if (!Double.TryParse(versionText, out latestVersion))
                    {
                        MessageBox.Show("Unable to parse: " + versionText);
                        return;
                    }

                    double currentVersion = 1.14;

                    if (currentVersion < latestVersion)
                    {
                        Update UpdateWindow = new Update();
                        UpdateWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        LaunchExecutable(LauncherFilePath);
                        Close();
                    }
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
    }
}