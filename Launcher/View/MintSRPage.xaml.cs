//using
#region
using DiscordRPC;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
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
        private void LaunchExecutable(string exePath)
        {
            MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            try
            {
                Process.Start(exePath);
                DiscordRPC();
                mainWindow.MinimizeToTray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching executable: {ex.Message}");
            }
        }
        #endregion
        //RPC
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1155013172382662677");

        public static void InitRPC()
        {
            client.OnReady += (sender, e) => { };

            client.OnPresenceUpdate += (sender, e) => { };

            client.OnError += (sender, e) => { };

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
                    LargeImageKey = "virus",
                    SmallImageKey = "starrail-logo",
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
            client.Invoke();
        }

        public static void DiscordRPC()
        {

            if (!client.IsInitialized)
            {
                InitRPC();
            }

            UpdateRPC();
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
        #endregion
    }
}
