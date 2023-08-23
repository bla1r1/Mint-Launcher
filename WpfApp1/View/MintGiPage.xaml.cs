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

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для MintGiPage.xaml
    /// </summary>
    public partial class MintGiPage : Page
    {
  
        public MintGiPage()
        {
            InitializeComponent();
            
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
            string serverFileUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/version.txt";
            string zipUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/minty.zip";
            int a = 1;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (File.Exists(verfilePath))
            {
                bool filesAreSame = await CheckIfFilesAreSameAsync(serverFileUrl, verfilePath);
                if (filesAreSame)
                {
                    if (File.Exists(launcherFilePath))
                    {
                        this.GI_button.Content = "Launch";
                        LaunchExecutable(launcherFilePath);
                        //mainWindow.MinimizeToTray();
                        //StartDiscordRPC();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    File.Delete(verfilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                    File.Delete(zipFilePath);
                    this.GI_button.Content = "Downloading";
                    await DownloadFile(zipUrl, zipFilePath);
                    await ExtractZipFile(zipFilePath, assetsFolderPath);
                    File.Delete(zipFilePath);
                    string fileContent = File.ReadAllText(verfilePath);
                    MessageBox.Show("Minty updated to version: " + fileContent,"Updated");
                    this.GI_button.Content = "Launch";
                }
            }
            else
            {
                this.GI_button.Content = "Downloading";
                Directory.CreateDirectory(assetsFolderPath);
                await DownloadFile(zipUrl, zipFilePath);
                await ExtractZipFile(zipFilePath, assetsFolderPath);
                File.Delete(zipFilePath);
                this.GI_button.Content = "Launch";
                LaunchExecutable(launcherFilePath);
                //mainWindow.MinimizeToTray();
                //StartDiscordRPC();
                Environment.Exit(0);
            }
        }




        //metods
        #region
        //download
        #region
        private async Task DownloadFile(string url, string destinationPath)
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
                    ZipFile.ExtractToDirectory(zipFilePath, extractionPath);
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
        private async Task<bool> CheckIfFilesAreSameAsync(string serverFileUrl,string localFilePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string serverFileContent = await client.DownloadStringTaskAsync(serverFileUrl);
                    string localFileContent = await ReadFileAsync(localFilePath);

                    return serverFileContent == localFileContent;
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

