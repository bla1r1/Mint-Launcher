namespace Minty
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Window_Loaded();
        }
        //Metods
        #region
        //CheckVerMetods
        #region
        private async void Window_Loaded()
        {
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LauncherFolderPath = System.IO.Path.Combine(MainFolderPath, "Launcher");
            string LauncherFilePath = System.IO.Path.Combine(LauncherFolderPath, "Launcher.exe");
            if (!File.Exists(LauncherFilePath))
            {
                Install InstallWIndow = new Install();
                InstallWIndow.Show();
                this.Close();
            }
            else
            {
                string onlineVersion = await GetOnlineVersionAsync();
                string fileVersion = GetFileVersionFromFile();

                if (onlineVersion == fileVersion)
                {
                    LaunchExecutable(LauncherFilePath);
                    Environment.Exit(0);
                }
                else
                {
                    Update UpdateWindow = new Update();
                    UpdateWindow.Show();
                    this.Close();
                }
            }
        }
        private async Task<string> GetOnlineVersionAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/Kinda-Wetty-Today/LauncherVer/main/LaunchVersion.json");

                if (response.IsSuccessStatusCode)
                {
                    string onlineVersion = await response.Content.ReadAsStringAsync();
                    return onlineVersion;
                }
                else
                {
                    throw new Exception("Failed to download online version.");
                }
            }
        }

        private string GetFileVersionFromFile()
        {
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LauncherFolderPath = System.IO.Path.Combine(MainFolderPath, "Launcher");
            string LauncherVersionPath = System.IO.Path.Combine(LauncherFolderPath, "LaunchVer.txt");
            string LauncherVersionFile = System.IO.File.ReadAllText(LauncherVersionPath);
            return LauncherVersionFile;
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