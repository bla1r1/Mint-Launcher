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

        public void MintyGIExist()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");

        }

        private async void launch_Click(object sender, RoutedEventArgs e)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipUrl = "https://github.com/kindawindytoday/Minty-Releases/releases/download/1.33/minty.zip";
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");
            string vertext = "1.33";
            string verUrl = "https://github.com/bla1r1/Version/releases/download/1.0/version.txt";
            
            if (File.Exists(launcherFilePath))
            {
                string verfileContent = File.ReadAllText(verfilePath);
                if (verfileContent.Contains(vertext))
                {
                    if (File.Exists(launcherFilePath))
                    {
                        this.GI_button.Content = "Launch";
                        LaunchExecutable(launcherFilePath);
                        //DiscordRPC();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    File.Delete(verfilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                    File.Delete(zipFilePath);
                    MessageBox.Show("Update Launcher");
                }
            }
            else
            {
                this.GI_button.Content = "Downloading";
                Directory.CreateDirectory(assetsFolderPath);
                await DownloadFile(verUrl, verfilePath);
                await DownloadFile(zipUrl, zipFilePath);
                await ExtractZipFile(zipFilePath, assetsFolderPath);
                File.Delete(zipFilePath);
                this.GI_button.Content = "Launch";
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


        //string TextBlockGIText = "хУУУУУУУУУУУУУУУУУУУУУУУУУУУУУУУй ";
        

        //public void changeText(string text)
        //{
        //    this.TextBlockGI.Text = "хУУУУУУУУУУУУУУУУй";
        //}

    }
}

