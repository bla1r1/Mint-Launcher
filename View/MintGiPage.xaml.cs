//using
#region
using System;
using System.Diagnostics;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO.Compression;
using static Minty.MainWindow;
using System.Windows.Media.Effects;
using FluentWpfChromes;
using System.Windows.Input;
using System.Windows.Threading;
using Octokit;
using Page = System.Windows.Controls.Page;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Minty.View
{
    /// <summary>
    /// Логика взаимодействия для MintGiPage.xaml
    /// </summary>
    public partial class MintGiPage : Page
    {
        private DispatcherTimer timer;
        private int currentProgress;
        public MintGiPage()
        {
            InitializeComponent();
            Timer();
            
        }
        //Metods
        #region
        //Launch
        #region
        public async void launch_Click(object sender, RoutedEventArgs e)
        {
            string accessToken = "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"; 
            string owner = "kindawindytoday";
            string repositoryName = "Minty-Releases";
            var client = new GitHubClient(new ProductHeaderValue("Launcher"));
            var tokenAuth = new Credentials(accessToken);
            client.Credentials = tokenAuth;
            var releases = await client.Repository.Release.GetAll(owner, repositoryName);
            var latestRelease = releases[0];
            var asset = latestRelease.Assets.FirstOrDefault();
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");
            string verFilePath = System.IO.Path.Combine(assetsFolderPath, "ver.txt");
            string verUrl = "https://github.com/rusya222/LauncherVer/blob/main/verGI.txt"; 
            string updateFilePath = "LauncherUpdater.exe";
            string versionUrllauncher = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
            string versionText = await DownloadVersionText(versionUrllauncher);
            MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            
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
                    LaunchExecutable(updateFilePath);
                    Environment.Exit(0);
                }
                else
                {

                    if (!File.Exists(launcherFilePath))
                    {
                        if (asset != null)
                        {
                            string downloadUrl = asset.BrowserDownloadUrl;

                            try
                            {
                                this.GI_button.Content = "Downloading";
                                Directory.CreateDirectory(assetsFolderPath);
                                Directory.CreateDirectory(mintyFolderPath);
                                    using (var webClient = new WebClient())
                                    {
                                        await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), zipFilePath);
                                        await webClient.DownloadFileTaskAsync(new Uri(verUrl), verFilePath);
                                    }
                                await ExtractZipFile(zipFilePath, assetsFolderPath);
                                File.Delete(zipFilePath);
                                this.GI_button.Content = "Launch";
                                LaunchExecutable(launcherFilePath);
                                mainWindow.MinimizeToTray();
                            }
                            #region//catch
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
                        else
                        {
                            MessageBox.Show("Minty.zip not found. The file name may not match.");
                        }
                    }
                    else
                    {             
                        try
                        {
                           string  VerText = File.ReadAllText(verFilePath);
                            Version localVersion;

                            if (Version.TryParse(VerText, out localVersion))
                            {
                                string githubVersionTag = latestRelease.TagName;
                                Version githubVersion;

                                if (Version.TryParse(githubVersionTag, out githubVersion))
                                {
                                    

                                    if (localVersion < githubVersion)
                                    {
                                        if (asset != null)
                                        {
                                            string downloadUrl = asset.BrowserDownloadUrl;

                                            try
                                            {
                                                #endregion
                                                this.GI_button.Content = "Downloading";
                                                File.Delete(verFilePath);
                                                File.Delete(launcherFilePath);
                                                File.Delete(dllFilePath);
                                                using (var webClient = new WebClient())
                                                {
                                                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), zipFilePath);
                                                    await webClient.DownloadFileTaskAsync(new Uri(verUrl), verFilePath);
                                                }
                                                await ExtractZipFile(zipFilePath, assetsFolderPath);
                                                File.Delete(zipFilePath);
                                                this.GI_button.Content = "Launch";
                                                string fileContent = File.ReadAllText(verFilePath);
                                                MessageBox.Show("Minty updated to version: " + fileContent, "Updated");
                                                LaunchExecutable(launcherFilePath);
                                                mainWindow.MinimizeToTray();
                                               
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
                                        else
                                        {
                                            MessageBox.Show("Minty.zip not found. The file name may not match.");
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Incorrect version format on GitHub: {githubVersionTag}");
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Incorrect version format in local file: {VerText}");
                            }
                        }

                        catch (WebException ex)
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
        #endregion
        //Download + Extract
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
        //CheckLauncherVer + CheckMintVer
        #region
        private async Task<string> DownloadVerfileContentAsync(string verfileUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(verfileUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Обработайте ошибку по вашему усмотрению.
                    Console.WriteLine("Ошибка при скачивании файла версии: " + ex.Message);
                }
                return string.Empty;
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
        //Progressbar
        #region
        private async Task Timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            currentProgress = 0;
            ProgressBar.Visibility = Visibility.Hidden;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value = currentProgress;
        }
        #endregion
        #endregion
    }
}