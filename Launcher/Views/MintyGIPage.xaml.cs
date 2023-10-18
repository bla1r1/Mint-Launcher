using System.Diagnostics.Eventing.Reader;

namespace Launcher.Views;
public sealed partial class MintyGIPage : Page
{
    public MintyGIViewModel ViewModel
    {
        get;
    }

    public MintyGIPage()
    {
        ViewModel = App.GetService<MintyGIViewModel>();
        InitializeComponent();
    }
    //metods
    #region
    //launch metod and click
    #region
    public async void Launch(object sender, RoutedEventArgs e)
    {
        string accessToken = "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";
        string owner = "kindawindytoday";
        string repositoryName = "Minty-Releases";
        var client = new GitHubClient(new Octokit.ProductHeaderValue("Launcher"));
        var tokenAuth = new Credentials(accessToken);
        client.Credentials = tokenAuth;

        var releases = await client.Repository.Release.GetAll(owner, repositoryName);
        Release? latestRelease = releases[0];


        if (latestRelease == null)
        {
            ShowErrorDialog("Unable to fetch the latest release.");
            return;
        }

        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string mintyFolderPath = Path.Combine(appDataFolder, "minty");
        string assetsFolderPath = Path.Combine(mintyFolderPath, "MintyGI");
        string launcherFilePath = Path.Combine(assetsFolderPath, "Launcher.exe");
        string dllFilePath = Path.Combine(assetsFolderPath, "minty.dll");
        string zipFilePath = Path.Combine(assetsFolderPath, "minty.zip");
        string verFilePath = Path.Combine(assetsFolderPath, "VerGI.txt");
        string verUrl = "https://raw.githubusercontent.com/Kinda-Wetty-Today/LauncherVer/main/VerGI.txt";

        if (!File.Exists(launcherFilePath))
        {
            if (latestRelease.Assets.Count == 0)
            {
                ShowErrorDialog("Minty.zip not found. The file name may not match.");
                return;
            }

            var asset = latestRelease.Assets[0];
            string downloadUrl = asset.BrowserDownloadUrl;

            GI_button.Content = "Downloading";

            Directory.CreateDirectory(assetsFolderPath);
            Directory.CreateDirectory(mintyFolderPath);

            bool downloadSuccess = await DownloadFilesAsync(downloadUrl, verUrl, zipFilePath, verFilePath, assetsFolderPath, launcherFilePath);

            if (downloadSuccess)
            {
                GI_button.Content = "Launch";
                LaunchExecutable(launcherFilePath);
            }
            else
            {
                ShowErrorDialog("Failed to download Minty.zip.");
            }
        }
        else
        {
            string verText = await File.ReadAllTextAsync(verFilePath);
            Version? localVersion;

            if (Version.TryParse(verText, out localVersion))
            {
                string githubVersionTag = latestRelease.TagName;
                Version? githubVersion;

                if (Version.TryParse(githubVersionTag, out githubVersion))
                {
                    if (localVersion < githubVersion)
                    {
                        if (latestRelease.Assets.Count == 0)
                        {
                            ShowErrorDialog("Minty.zip not found. The file name may not match.");
                            return;
                        }

                        var asset = latestRelease.Assets[0];
                        string downloadUrl = asset.BrowserDownloadUrl;

                        GI_button.Content = "Downloading";

                        File.Delete(verFilePath);
                        File.Delete(launcherFilePath);
                        File.Delete(dllFilePath);

                        bool downloadSuccess = await DownloadFilesAsync(downloadUrl, verUrl, zipFilePath, verFilePath, assetsFolderPath, launcherFilePath);

                        if (downloadSuccess)
                        {
                            GI_button.Content = "Launch";
                            ShowInformationDialog($"Minty updated to version: {await File.ReadAllTextAsync(verFilePath)}");
                            LaunchExecutable(launcherFilePath);
                        }
                    }
                    else
                    {
                        GI_button.Content = "Launch";
                        LaunchExecutable(launcherFilePath);
                    }
                }
                else
                {
                    ShowErrorDialog($"Incorrect version format on GitHub: {githubVersionTag}");
                }
            }
            else
            {
                ShowErrorDialog($"Incorrect version format in local file: {verText}");
            }
        }
    }

    public void LaunchExecutable(string exePath)
    {
        try
        {
            DiscordRPC();
            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.EnableRaisingEvents = true;

            process.Exited += new EventHandler(Process_Exited);
            process.Start();
        }
        catch (Exception ex)
        {
            ShowErrorDialog($"Error launching executable: {ex.Message}");
        }
    }
    static void Process_Exited(object sender, EventArgs e)
    {
       MainWindow.DiscordRPC();
    }
    #endregion
    //Download and extract metods
    #region
    public async Task<bool> DownloadFilesAsync(string downloadUrl, string verUrl, string zipFilePath, string verFilePath, string assetsFolderPath, string launcherFilePath)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                async Task WriteDownloadedBytesToDisk(byte[] content, string filePath)
                {
                    await File.WriteAllBytesAsync(filePath, content);
                }
                using (var downloadTask = httpClient.GetByteArrayAsync(downloadUrl))
                {
                    using (var verTask = httpClient.GetByteArrayAsync(verUrl))
                    {
                        var tasks = new[] { downloadTask, verTask };
                        await Task.WhenAll(tasks);
                        foreach (var task in tasks)
                        {
                            task.Dispose();
                        }
                        await WriteDownloadedBytesToDisk(downloadTask.Result, zipFilePath);
                        await WriteDownloadedBytesToDisk(verTask.Result, verFilePath);
                    }
                }
            }

            await ExtractZipFile(zipFilePath, assetsFolderPath);
            File.Delete(zipFilePath);
            return true;
        }
        catch (HttpRequestException ex)
        {
            ShowErrorDialog($"Error downloading file: {ex.Message}");
        }
        catch (IOException ex)
        {
            ShowErrorDialog($"Error saving file: {ex.Message}");
        }
        catch (Exception ex)
        {
            ShowErrorDialog($"An unexpected error occurred: {ex.Message}");
        }

        return false;
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
            ShowErrorDialog($"Error while extracting the archive: {ex.Message}");
        }
    }
    #endregion
    //Error or information metods
    #region
    public async void ShowInformationDialog(string content)
    {
        await ShowInformationDialog("Information", content);
    }

    public async Task ShowInformationDialog(string title, string content)
    {
        ContentDialog errorDialog = new()
        {
            Title = title,
            Content = content,
            CloseButtonText = "Ok",
            XamlRoot = this.XamlRoot
        };
        await errorDialog.ShowAsync();
    }

    public async void ShowErrorDialog(string message)
    {
        await ShowInformationDialog("Error", message);
    }
    #endregion
    //Rcp
    #region
    private static readonly DiscordRpcClient client = new DiscordRpcClient("1112360491847778344");

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
                LargeImageKey = "idol",
                SmallImageKey = "gensh",
                SmallImageText = "Genshin Impact"
            },
            Buttons = new DiscordRPC.Button[]
            {
                    new DiscordRPC.Button()
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
#endregion
}


