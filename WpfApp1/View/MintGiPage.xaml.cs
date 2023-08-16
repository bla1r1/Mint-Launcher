using System;
using System.Diagnostics;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using Button = DiscordRPC.Button;
using Newtonsoft.Json;
using System.IO.Compression;
using static WpfApp1.MainWindow;
using DiscordRPC;
using Octokit;
using System.Collections.Generic;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для MintGiPage.xaml
    /// </summary>
    public partial class MintGiPage : System.Windows.Controls.Page
    {
        string owner = "название_владельца"; // Например, "username"
        string repo = "название_репозитория"; // Например, "my-private-repo"
        string token = "ваш_персональный_токен"; // Замените на свой токен


        public MintGiPage()
        {
            InitializeComponent();
        }

        public void MintyGIExist()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            if (File.Exists(launcherFilePath))
            {
                GI_button.Content = "хУУУУУУУУУУУУУУУУй";
            }
            else
            {
                GI_button.Content = "Downoad";
            }
        }
        
        private async void launch_Click(object sender, RoutedEventArgs e)
        {
            var client = new GitHubClient(new ProductHeaderValue("GitHubReleaseDownloader"));
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
            var releases = await client.Repository.Release.GetAll(owner, repo);
            var latestRelease = releases[0]; 
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");
                    if (File.Exists(launcherFilePath))
                    {
                    LaunchExecutable(launcherFilePath);
                    DiscordRPC();
                    System.Windows.Application.Current.Shutdown();
                    }
                        else
                        {
                        Directory.CreateDirectory(assetsFolderPath);
                        await Task.Delay(2000);
                        MessageBox.Show("File Downloaded");
                        await ExtractZipFile(zipFilePath, assetsFolderPath);
                        await Task.Delay(2000);
                        LaunchExecutable(launcherFilePath);
                        DiscordRPC();
                        System.Windows.Application.Current.Shutdown();
                        }
                    }

                  

        private async Task DownloadFile(string url, string destinationPath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (FileStream fileStream = new FileStream(destinationPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

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
        }

        //ReadVersion.txt
        #region

        #endregion


        //RPC
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1112360491847778344");
        public static void InitRPC()
        {
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
        }
        #endregion
        public void DiscordRPC()
        {
            InitRPC();
            UpdateRPC();
            for (; ; );
        }

        private async Task<string> DownloadVersionText(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Unable to connect to the web server.");
                    return null;
                }

                string versionText = await response.Content.ReadAsStringAsync();
                return versionText;
            }
        }

        private async Task ExtractZipFile(string zipFilePath, string extractionPath)
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
                MessageBox.Show("Error while extracting the archive: " + ex.Message);
            }
        }
        private async Task UpdateLauncher(string url, string destinationPath)
        {
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(destinationPath));

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (FileStream fileStream = new FileStream(tempFilePath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                    File.Move(tempFilePath, destinationPath);
                    MessageBox.Show("File downloaded successfully!");
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
        }

        static async Task DownloadReleaseAssetsAsync(string token, IReadOnlyList<ReleaseAsset> assets, string localPath)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Authorization", $"token {token}");

                foreach (var asset in assets)
                {
                    string downloadUrl = asset.BrowserDownloadUrl;
                    string assetName = asset.Name;
                    string fullPath = Path.Combine(localPath, assetName);

                    await webClient.DownloadFileTaskAsync(downloadUrl, fullPath);

                    Console.WriteLine($"Downloaded {assetName}");
                }
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


        string TextBlockGIText = "хУУУУУУУУУУУУУУУУУУУУУУУУУУУУУУУй ";
        

        //public void changeText(string text)
        //{
        //    this.TextBlockGI.Text = "хУУУУУУУУУУУУУУУУй";
        //}

    }
}

