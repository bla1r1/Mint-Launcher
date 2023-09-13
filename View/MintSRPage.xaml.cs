//using
#region
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
using Octokit;
using Page = System.Windows.Controls.Page;
#endregion
namespace Minty.View
{
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
            string accessToken = "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";
            string owner = "kindawindytoday";
            string repositoryName = "Minty-SR-Releases";
            var client = new GitHubClient(new ProductHeaderValue("LauncherSR"));
            var tokenAuth = new Credentials(accessToken);
            client.Credentials = tokenAuth;
            var releases = await client.Repository.Release.GetAll(owner, repositoryName);
            var latestRelease = releases[0];
            var asset = latestRelease.Assets.FirstOrDefault();
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintySR");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "mintySR.zip");
            string verFilePath = System.IO.Path.Combine(assetsFolderPath, "verSR.txt");
            string verUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/VerSR.txt";
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
                                this.HSR_button.Content = "Downloading";
                                Directory.CreateDirectory(assetsFolderPath);
                                Directory.CreateDirectory(mintyFolderPath);
                                using (var webClient = new WebClient())
                                {
                                    await webClient.DownloadFileTaskAsync(new Uri(downloadUrl), zipFilePath);
                                    await webClient.DownloadFileTaskAsync(new Uri(verUrl), verFilePath);
                                }
                                await ExtractZipFile(zipFilePath, assetsFolderPath);
                                File.Delete(zipFilePath);
                                this.HSR_button.Content = "Launch";
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


                                    if (localVersion < githubVersion)
                                    {
                                        if (asset != null)
                                        {
                                            string downloadUrl = asset.BrowserDownloadUrl;

                                            try
                                            {

                                                this.HSR_button.Content = "Downloading";
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
                                                this.HSR_button.Content = "Launch";
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
                                    else
                                    {
                                        LaunchExecutable(launcherFilePath);
                                        mainWindow.MinimizeToTray();
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
        #endregion
        //download
        #region

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
