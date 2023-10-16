namespace Minty.View
{
    public partial class Install : Window
    {
        private DispatcherTimer timer;
        private int currentProgress;
        public Install()
        {
            InitializeComponent();
            Timer();
        }
        //Metods
        #region
        //Install Click
        #region
        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LauncherFolderPath = System.IO.Path.Combine(MainFolderPath, "Launcher");
            string LauncherFilePath = System.IO.Path.Combine(LauncherFolderPath, "Launcher.exe");
            string LauncherZipFilePath = System.IO.Path.Combine(LauncherFolderPath, "Launcher.zip");
            string LauncherVersionPath = System.IO.Path.Combine(LauncherFolderPath, "LaunchVer.txt");
            string LauncherVerUrl = "https://raw.githubusercontent.com/Kinda-Wetty-Today/LauncherVer/main/LaunchVersion.txt";
            string LauncherUrl = "https://github.com/Kinda-Wetty-Today/LauncherVer/releases/download/1.0/Minty.zip";
            Directory.CreateDirectory(LauncherFolderPath);
            Button.Visibility = Visibility.Hidden;
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            timer.Start();
            await DownloadFile(LauncherUrl, LauncherZipFilePath);
            await DownloadFile(LauncherVerUrl, LauncherVersionPath);
            await ExtractZipFile(LauncherZipFilePath, LauncherFolderPath);
            File.Delete(LauncherZipFilePath);
            LaunchExecutable(LauncherFilePath);
            ProgressBar.Visibility = Visibility.Hidden;
            Button.Visibility = Visibility.Visible;
            Environment.Exit(0);    
        }
        #endregion
        //Progressbar
        #region
        private Task Timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            currentProgress = 0;
            ProgressBar.Visibility = Visibility.Hidden;
            return Task.CompletedTask;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value = currentProgress;
        }
        #endregion
        //Download and Extract
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
        //DragMove and Close
        #region
        private void Close(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
        #endregion

    }
}
