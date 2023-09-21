using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Page = Microsoft.UI.Xaml.Controls.Page;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO.Compression;
using CommunityToolkit.WinUI.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Launcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MintGIPage : Page
    {
        public MintGIPage()
        {
            this.InitializeComponent();
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
            string verFilePath = System.IO.Path.Combine(assetsFolderPath, "verGI.txt");
            string verUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/verGI.txt";
            string updateFilePath = "LauncherUpdater.exe";
            string versionUrllauncher = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
            string versionText = await DownloadVersionText(versionUrllauncher);


            if (versionText != null)
            {
                double latestVersion = .0;

                if (!Double.TryParse(versionText, out latestVersion))
                {
                    ContentDialog ErrorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Unable to parse: " + versionText,
                        CloseButtonText = "Ok"
                    };

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

                            }

                            catch (HttpRequestException ex)
                            {
                                ContentDialog ErrorDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = "Error downloading file: " + ex.Message,
                                    CloseButtonText = "Ok"
                                };
                            }
                            catch (IOException ex)
                            {
                                ContentDialog ErrorDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = "Error saving file: " + ex.Message,
                                    CloseButtonText = "Ok"
                                };

                            }
                            catch (Exception ex)
                            {
                                ContentDialog ErrorDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = "An unexpected error occurred: " + ex.Message,
                                    CloseButtonText = "Ok"
                                };

                            }
                        }
                        else
                        {
                            ContentDialog ErrorDialog = new ContentDialog
                            {
                                Title = "Error",
                                Content = "Minty.zip not found. The file name may not match.",
                                CloseButtonText = "Ok"
                            };
                        }
                    }
                    else
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
                                            ContentDialog UpdateDialog = new ContentDialog
                                            {
                                                Title = "Updated",
                                                Content = "Minty updated to version: " + fileContent,
                                                CloseButtonText = "Ok"
                                            };

                                            LaunchExecutable(launcherFilePath);


                                        }
                                        catch (HttpRequestException ex)
                                        {
                                            ContentDialog ErrorDialog = new ContentDialog
                                            {
                                                Title = "Error",
                                                Content = "Error downloading file: " + ex.Message,
                                                CloseButtonText = "Ok"
                                            };
                                        }
                                        catch (IOException ex)
                                        {
                                            ContentDialog ErrorDialog = new ContentDialog
                                            {
                                                Title = "Error",
                                                Content = "Error saving file: " + ex.Message,
                                                CloseButtonText = "Ok"
                                            };

                                        }
                                        catch (Exception ex)
                                        {
                                            ContentDialog ErrorDialog = new ContentDialog
                                            {
                                                Title = "Error",
                                                Content = "An unexpected error occurred: " + ex.Message,
                                                CloseButtonText = "Ok"
                                            };

                                        }
                                    }

                                    else
                                    {
                                        ContentDialog ErrorDialog = new ContentDialog
                                        {
                                            Title = "Error",
                                            Content = "Minty.zip not found. The file name may not match.",
                                            CloseButtonText = "Ok"
                                        };

                                    }
                                }
                                else
                                {
                                    LaunchExecutable(launcherFilePath);

                                }
                            }
                            else
                            {
                                ContentDialog ErrorDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = "Incorrect version format on GitHub: " + githubVersionTag,
                                    CloseButtonText = "Ok"
                                };

                            }
                        }
                        else
                        {
                            ContentDialog ErrorDialog = new ContentDialog
                            {
                                Title = "Error",
                                Content = "Incorrect version format in local file: " + VerText,
                                CloseButtonText = "Ok"
                            };

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
                ContentDialog ErrorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Error launching executable: " + ex.Message,
                    CloseButtonText = "Ok"
                };
            }
        }
        #endregion
        //Extract
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
                ContentDialog ErrorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Error while extracting the archive: " + ex.Message,
                    CloseButtonText = "Ok"
                };
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
                    ContentDialog ErrorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Error while downloading version file:" + ex.Message,
                        CloseButtonText = "Ok"
                    };
                    
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
                    ContentDialog ErrorDialog = new ContentDialog
                    {
                        Title = "No Internet Connection.",
                        Content = "Unable to connect to the web server.",
                        CloseButtonText = "Ok"
                    };
                    
                }

                string versionText = await response.Content.ReadAsStringAsync();
                return versionText;
            }
        }


        #endregion
        #endregion
    }
}
