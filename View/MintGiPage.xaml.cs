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
            string verUrl = "https://github.com/rusya222/LauncherVer/raw/main/ver.txt"; 
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
                                Directory.CreateDirectory(assetsFolderPath);
                                Directory.CreateDirectory(mintyFolderPath);
                                using (var webClient = new WebClient())
                                {
                                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), zipFilePath);
                                }   
                                await DownloadFile(verUrl, verFilePath);
                                //await ExtractZipFile(zipFilePath, assetsFolderPath);
                                //File.Delete(zipFilePath);
                                //LaunchExecutable(launcherFilePath);
                                //mainWindow.MinimizeToTray();
                            }
                            catch (WebException ex)
                            {  
                                MessageBox.Show("Произошла ошибка при скачивании файла: " + ex.Message);
                            }
                            catch (Exception ex)
                            {    
                                MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Asset не найден. Возможно, имя файла не совпадает.");
                        }
                    }
                    else
                    {
                        try
                        {
                            string VerText = File.ReadAllText(verFilePath);
                            Version localVersion;

                            if (Version.TryParse(VerText, out localVersion))
                            {
                                string githubVersionTag = latestRelease.TagName;
                                Version githubVersion;

                                if (Version.TryParse(githubVersionTag, out githubVersion))
                                {
                                    int versionComparison = githubVersion.CompareTo(localVersion);

                                    if (versionComparison < 0)
                                    {
                                        MessageBox.Show("Локальная версия устарела.");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Локальная версия и версия на GitHub равны.");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Некорректный формат версии на GitHub: {githubVersionTag}");
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Некорректный формат версии в локальном файле: {VerText}");
                            }
                        }

                        catch (WebException ex)
                        {
                            // Обработка ошибки сетевого запроса, если она возникла
                            MessageBox.Show("Произошла ошибка при скачивании файла: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            // Обработка других исключений, если они возникли
                            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
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
                        long totalBytes = response.Content.Headers.ContentLength ?? -1;
                        long downloadedBytes = 0;
                        byte[] buffer = new byte[8192]; // Размер буфера для считывания данных

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            int bytesRead;
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                downloadedBytes += bytesRead;
                                currentProgress = (int)((downloadedBytes * 100) / totalBytes);
                            }
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
            finally
            {
                timer.Stop();
                ProgressBar.Visibility = Visibility.Hidden;
                currentProgress = 0;
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