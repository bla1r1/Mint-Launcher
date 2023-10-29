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
        Random random = new Random();
        int token = random.Next(1, 3);
        string? accessToken = null;
        if (token == 1){accessToken = "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";}
        else if (token == 2){accessToken = "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";}
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
        string mintyFolderPath = Path.Combine(appDataFolder, "Minty");
        string assetsFolderPath = Path.Combine(mintyFolderPath, "MintyGI");
        string launcherFilePath = Path.Combine(assetsFolderPath, "Launcher.exe");
        string dllFilePath = Path.Combine(assetsFolderPath, "minty.dll");
        string zipFilePath = Path.Combine(assetsFolderPath, "minty.zip");
        string verFilePath = Path.Combine(assetsFolderPath, "VerGI");
        string latestReleaseTag = latestRelease.TagName;
     

        if (!File.Exists(verFilePath))
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

                bool downloadSuccess = await DownloadFilesAsync(downloadUrl,zipFilePath, assetsFolderPath, launcherFilePath);
                using (StreamWriter writer = new StreamWriter(verFilePath))
                {
                    await writer.WriteLineAsync(latestReleaseTag);
                }
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

            if (!Version.TryParse(verText, out localVersion))
            {
                ShowErrorDialog($"Incorrect version format in local file: {verText}");
                return;
            }

            string githubVersionTag = latestRelease.TagName;
            Version? githubVersion;

            if (!Version.TryParse(githubVersionTag, out githubVersion))
            {
                ShowErrorDialog($"Incorrect version format on GitHub: {githubVersionTag}");
                return;
            }
            if (latestRelease.Assets.Count == 0)
            {
                ShowErrorDialog("Minty.zip not found. The file name may not match.");
                return;
            }

            if (localVersion >= githubVersion)
            {
                GI_button.Content = "Launch";
                LaunchExecutable(launcherFilePath);
                return;
            }

            if (localVersion == githubVersion)
            {
                var asset = latestRelease.Assets[0];
                string downloadUrl = asset.BrowserDownloadUrl;

                GI_button.Content = "Downloading";

                File.Delete(verFilePath);
                File.Delete(launcherFilePath);
                File.Delete(dllFilePath);

                bool downloadSuccess = await DownloadFilesAsync(downloadUrl, zipFilePath, assetsFolderPath, launcherFilePath);
                using (StreamWriter writer = new StreamWriter(verFilePath))
                {
                    await writer.WriteLineAsync(latestReleaseTag);
                }
                if (downloadSuccess)
                {
                    GI_button.Content = "Launch";
                    ShowInformationDialog($"Minty updated to version: {await File.ReadAllTextAsync(verFilePath)}");
                    LaunchExecutable(launcherFilePath);
                }
                return;
            }
        }
    }

    public void LaunchExecutable(string exePath)
    {
        try
        {
            UpdateRPC("Minty", "Hacking MHY <333");
            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.EnableRaisingEvents = true;
            process.Start();
        }
        catch (Exception ex)
        {
           ShowErrorDialog($"Error launching executable: {ex.Message}");
        }
    }
    #endregion
    //Download and Extract
    #region
    public async Task<bool> DownloadFilesAsync(string downloadUrl,string zipFilePath, string assetsFolderPath, string launcherFilePath)
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
                        var tasks = new[] { downloadTask};
                        await Task.WhenAll(tasks);
                        foreach (var task in tasks)
                        {
                            task.Dispose();
                        }
                        await WriteDownloadedBytesToDisk(downloadTask.Result, zipFilePath);
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
    //Error Dialog
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

    public static void InitializeDiscordRPC()
    {
        if (!client.IsInitialized)
        {
            client.OnReady += (sender, e) => { };

            client.OnPresenceUpdate += (sender, e) => { };

            client.OnError += (sender, e) => { };

            client.Initialize();
        }
    }

    public static void UpdateRPC(string state, string details)
    {
        InitializeDiscordRPC();

        var presence = new RichPresence()
        {
            State = state,
            Details = details,
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
    }
    #endregion
    #endregion
}


