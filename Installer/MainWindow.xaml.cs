﻿namespace Minty
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded();
        }
        //Metods
        #region
        //CheckVerMetods
        #region
        private new async void Loaded()
        {
            Random random = new Random();
            int token = random.Next(1, 3);
            string? accessToken = null;
            if (token == 1) { accessToken = "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"; }
            else if (token == 2) { accessToken = "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"; }
            string owner = "Kinda-Wetty-Today";
            string repositoryName = "Minty-Launcher-Releases";
            var client = new GitHubClient(new Octokit.ProductHeaderValue("Launcher"));
            var tokenAuth = new Credentials(accessToken);
            client.Credentials = tokenAuth;

            var releases = await client.Repository.Release.GetAll(owner, repositoryName);
            Release? latestRelease = releases[0];
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LauncherFolderPath = System.IO.Path.Combine(MainFolderPath, "Launcher");
            string LauncherFilePath = System.IO.Path.Combine(LauncherFolderPath, "Launcher.exe");
            string verFilePath = Path.Combine(LauncherFolderPath, "LauncherVer");
            string LogFilePath = System.IO.Path.Combine(MainFolderPath, "Launcherlog");
            if (File.Exists(LogFilePath))
            {
            }
            else
            {
                    Video VideoWIndow = new Video();
                    VideoWIndow.Show();
                    this.Close();
                    return;
                    
            }
            if (latestRelease == null)
            {
                MessageBox.Show("Unable to fetch the latest release.");
                return;
            }
            

            if (latestRelease.Assets.Count == 0)
            {
                MessageBox.Show("Minty.zip not found. The file name may not match.");
                return;
            }
            if (!File.Exists(verFilePath))
            {
                Install InstallWIndow = new Install();
                InstallWIndow.Show();
                this.Close();
            } 
            else
            {
                string verText = await File.ReadAllTextAsync(verFilePath);
                Version? localVersion;

                if (!Version.TryParse(verText, out localVersion))
                {
                    MessageBox.Show($"Incorrect version format in local file: {verText}");
                    return;
                }

                string githubVersionTag = latestRelease.TagName;
                Version? githubVersion;

                if (!Version.TryParse(githubVersionTag, out githubVersion))
                {
                    MessageBox.Show($"Incorrect version format on GitHub: {githubVersionTag}");
                    return;
                }

                if (localVersion >= githubVersion)
                {
                    LaunchExecutable(LauncherFilePath);
                    Environment.Exit(0);
                    return;
                }

                if (latestRelease.Assets.Count == 0)
                {
                    MessageBox.Show("Minty.zip not found. The file name may not match.");
                    return;
                }
                Update UpdateWindow = new Update();
                UpdateWindow.Show();
                this.Close();
            }
        }
        #endregion
        //Launch
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
        #endregion
    }
}