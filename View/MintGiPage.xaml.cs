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
        //Backgroundvideo
        #region
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Ошибка при воспроизведении медиа: " + e.ErrorException.Message);
        }
        #endregion
        //Launch
        #region
        public async void launch_Click(object sender, RoutedEventArgs e)
        {

            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "mintyGI.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");
            string updateFilePath = "LauncherUpdater.exe";
            string serverFileUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/versionGi.txt";
            string zipUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/mintyGI.zip";
            string updateUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/update.exe";
            string versionUrl = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
            string versionText = await DownloadVersionText(versionUrl);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
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
                    if (!File.Exists(verfilePath))
                    {
                        GI_button.Visibility = Visibility.Hidden;
                        ProgressBar.Visibility = Visibility.Visible;
                        ProgressBar.Value = 0;
                        timer.Start();
                        Directory.CreateDirectory(assetsFolderPath);
                        Directory.CreateDirectory(mintyFolderPath);
                        await DownloadFile(zipUrl, zipFilePath);
                        await ExtractZipFile(zipFilePath, assetsFolderPath);
                        File.Delete(zipFilePath);
                        ProgressBar.Visibility = Visibility.Hidden;
                        GI_button.Visibility = Visibility.Visible;
                        LaunchExecutable(launcherFilePath);
                        mainWindow.MinimizeToTray();
                    }
                    else
                    {
                        bool filesAreSame = await CheckIfFilesAreSameAsync(serverFileUrl, verfilePath);
                        if (filesAreSame)
                        {
                            if (File.Exists(launcherFilePath))
                            {
                                
                                LaunchExecutable(launcherFilePath);
                                mainWindow.MinimizeToTray();
                            }
                        }
                        else
                        {
                            GI_button.Visibility = Visibility.Hidden;
                            ProgressBar.Visibility = Visibility.Visible;
                            ProgressBar.Value = 0;
                            timer.Start();
                            File.Delete(dllFilePath);
                            File.Delete(verfilePath);
                            File.Delete(launcherFilePath);
                            await DownloadFile(zipUrl, zipFilePath);
                            await ExtractZipFile(zipFilePath, assetsFolderPath);
                            File.Delete(zipFilePath);
                            string fileContent = File.ReadAllText(verfilePath);
                            ProgressBar.Visibility = Visibility.Hidden;
                            GI_button.Visibility = Visibility.Visible;
                            MessageBox.Show("Minty updated to version: " + fileContent, "Updated");
                            mainWindow.MinimizeToTray();
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

                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
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
        private async Task<bool> CheckIfFilesAreSameAsync(string serverFileUrl, string localFilePath)
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

        private async Task<string> ReadFileAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
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