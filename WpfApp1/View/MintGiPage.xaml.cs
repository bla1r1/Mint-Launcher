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
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Windows.Ink;
using Windows.Security.Cryptography.Certificates;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;


namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для MintGiPage.xaml
    /// </summary>
    public partial class MintGiPage : Page
    {

        private readonly HttpClient _httpClient;
  
        public MintGiPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Minty-Releases"); // Required by GitHub API
        }


        public async void launch_Click(object sender, RoutedEventArgs e)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");

            //string serverFileUrl = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/MintGIVersion";
            //string zipUrl = "http://138.2.145.17/minty.zip";
            string serverFileUrl = "";
            string zipUrl = "";

            try
            {
                var (version, assetUrl, changelog) = await GetLatestReleaseInfoAsync();
                serverFileUrl = version;
                zipUrl = assetUrl;
                this.TextBlockGI.Text = changelog;
                this.debugtext.AppendText("Latest Version: " + serverFileUrl);
                this.debugtext.AppendText(Environment.NewLine);
                this.debugtext.AppendText("Url: " + zipUrl);
                this.debugtext.AppendText(Environment.NewLine);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


           


            if (File.Exists(verfilePath))
            {
                this.debugtext.AppendText("File Exists?: " + File.Exists(verfilePath));
                this.debugtext.AppendText(Environment.NewLine);
                bool filesAreSame = await CheckIfFilesAreSameAsync(serverFileUrl, verfilePath);

                this.debugtext.AppendText("is the Same?: " + filesAreSame);
                this.debugtext.AppendText(Environment.NewLine);
                if (filesAreSame)
                {
                    if (File.Exists(launcherFilePath))
                    {
                        this.GI_button.Content = "Launch";
                        LaunchExecutable(launcherFilePath);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    progressBar.Value = 0;
                    File.Delete(verfilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                    File.Delete(zipFilePath);
                    
                }
            }
            else
            {
                this.GI_button.Content = "Downloading";
                Directory.CreateDirectory(assetsFolderPath);
                this.debugtext.AppendText("Asset Directory: " + assetsFolderPath);
                this.debugtext.AppendText(Environment.NewLine);
                var progress = new Progress<int>(value => progressBar.Value = value);

                await DownloadFile(zipUrl, zipFilePath, progress);
                //aaa
                await ExtractZipFile(zipFilePath, assetsFolderPath);
                File.Delete(zipFilePath);
                this.GI_button.Content = "Launch";
                progressBar.Value = 0;
            }
        }


        public async Task<(string Version, string AssetUrl, string Changelog)> GetLatestReleaseInfoAsync()
        {
            var requestUri = "https://api.github.com/repos/kindawindytoday/Minty-Releases/releases/latest";

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseContent);

            var version = jsonResponse["tag_name"].ToString();
            var changelog = jsonResponse["body"].ToString();
            var assetUrl = jsonResponse["assets"][0]["browser_download_url"].ToString();
            
            return (version, assetUrl, changelog);
        }


        //metods
        #region
        //download
        #region
        private async Task DownloadFile(string url, string destinationPath, IProgress<int> progress = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1;
                    var bytesRead = 0;

                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            var buffer = new byte[8192];
                            int bytesReadLast;

                            while ((bytesReadLast = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesReadLast);
                                bytesRead += bytesReadLast;

                                if (progress != null && totalBytes > 0)
                                {
                                    progress.Report((int)((float)bytesRead / totalBytes * 100));
                                }
                            }
                            //await response.Content.CopyToAsync(fileStream);
                        }
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
        public void DiscordRPC()
        {
            InitRPC();
            UpdateRPC();
            for (; ; );
        }
        #endregion
        //EXTRACT
        #region
        private async Task ExtractZipFile(string zipFilePath, string extractionPath)
        {
            try
            {
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(zipFilePath, extractionPath, true);
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while extracting the archive: " + ex.Message);
            }
        }
        #endregion
        //launch
        #region
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
        //checkGiver
        #region
        private async Task<bool> CheckIfFilesAreSameAsync(string serverVersion,string localFilePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    //string serverFileContent = await client.DownloadStringTaskAsync(serverFileUrl);
                    string localVersion = await ReadFileAsync(localFilePath);
                    this.debugtext.AppendText("Local version:" + localVersion);
                    this.debugtext.AppendText(Environment.NewLine);
                    //return serverFileContent == localFileContent;
                    return serverVersion == localVersion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking files: {ex.Message}");
                return false;
            }
        }

        private async Task<string> ReadFileAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }
        #endregion
        #endregion
    }
}

