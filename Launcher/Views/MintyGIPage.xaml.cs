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
            ShellPage.ShowErrorDialog("Unable to fetch the latest release.");
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
                ShellPage.ShowErrorDialog("Minty.zip not found. The file name may not match.");
                return;
            }

            var asset = latestRelease.Assets[0];
            string downloadUrl = asset.BrowserDownloadUrl;

            GI_button.Content = "Downloading";

            Directory.CreateDirectory(assetsFolderPath);
            Directory.CreateDirectory(mintyFolderPath);

            bool downloadSuccess = await DownloadService.DownloadFilesAsync(downloadUrl, verUrl, zipFilePath, verFilePath, assetsFolderPath, launcherFilePath);

            if (downloadSuccess)
            {
                GI_button.Content = "Launch";
                LaunchExecutable(launcherFilePath);
            }
            else
            {
                ShellPage.ShowErrorDialog("Failed to download Minty.zip.");
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
                            ShellPage.ShowErrorDialog("Minty.zip not found. The file name may not match.");
                            return;
                        }

                        var asset = latestRelease.Assets[0];
                        string downloadUrl = asset.BrowserDownloadUrl;

                        GI_button.Content = "Downloading";

                        File.Delete(verFilePath);
                        File.Delete(launcherFilePath);
                        File.Delete(dllFilePath);

                        bool downloadSuccess = await DownloadService.DownloadFilesAsync(downloadUrl, verUrl, zipFilePath, verFilePath, assetsFolderPath, launcherFilePath);

                        if (downloadSuccess)
                        {
                            GI_button.Content = "Launch";
                            ShellPage.ShowInformationDialog($"Minty updated to version: {await File.ReadAllTextAsync(verFilePath)}");
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
                    ShellPage.ShowErrorDialog($"Incorrect version format on GitHub: {githubVersionTag}");
                }
            }
            else
            {
                ShellPage.ShowErrorDialog($"Incorrect version format in local file: {verText}");
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
           ShellPage.ShowErrorDialog($"Error launching executable: {ex.Message}");
        }
    }
    static void Process_Exited(object sender, EventArgs e)
    {
        client.Dispose();
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


