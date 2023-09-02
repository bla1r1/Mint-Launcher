//using
#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
#endregion
namespace Minty.View
{
    /// <summary>
    /// Логика взаимодействия для MintHsrPage.xaml
    /// </summary>
    public partial class MintHsrPage : Page
    {
        public MintHsrPage()
        {
            InitializeComponent();
        }
        //metods
        #region
        //launch
        #region
        public async void launch_Click(object sender, RoutedEventArgs e)
        {

            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyHSR");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "mintyHSR.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "versionHSR.txt");
            string tempFolderPath = System.IO.Path.GetTempPath();
            string updateFilePath = System.IO.Path.Combine(tempFolderPath, "update.exe");
            string serverFileUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/versionHSR.txt";
            string zipUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/mintyHSr.zip";
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

                double currentVersion = 1.13;

                if (currentVersion < latestVersion)
                {
                    await DownloadFile(updateUrl, updateFilePath);
                    MessageBox.Show($"Launcher got an update.", "Update");
                    LaunchExecutable(updateFilePath);
                    Environment.Exit(0);
                }
                else
                {
                    if (!File.Exists(verfilePath))
                    {
                        this.HSR_button.Content = "Downloading";
                        Directory.CreateDirectory(assetsFolderPath);
                        Directory.CreateDirectory(mintyFolderPath);
                        await DownloadFile(zipUrl, zipFilePath);
                        await ExtractZipFile(zipFilePath, assetsFolderPath);
                        File.Delete(zipFilePath);
                        this.HSR_button.Content = "Launch";
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
                                this.HSR_button.Content = "Launch";
                                LaunchExecutable(launcherFilePath);
                                mainWindow.MinimizeToTray();
                            }
                        }
                        else
                        {
                            this.HSR_button.Content = "Downloading";
                            Directory.CreateDirectory(assetsFolderPath);
                            Directory.CreateDirectory(mintyFolderPath);
                            await DownloadFile(zipUrl, zipFilePath);
                            await ExtractZipFile(zipFilePath, assetsFolderPath);
                            string fileContent = File.ReadAllText(verfilePath);
                            MessageBox.Show("Minty updated to version: " + fileContent, "Updated");
                            this.HSR_button.Content = "Launch";
                            mainWindow.MinimizeToTray();
                        }
                    }
                }
            }
        }
        #endregion
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
        //checkver
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
        #endregion
    }
}
