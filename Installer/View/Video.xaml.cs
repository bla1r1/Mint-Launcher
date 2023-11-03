namespace Minty.View
{
    public partial class Video : Window
    {
        public Video()
        {
            InitializeComponent();
            Video1();

        }
        public async Task Video1()
        {
            string MainFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string LogFilePath = System.IO.Path.Combine(MainFolderPath, "Launcherlog");
            string VideoFilePath = System.IO.Path.Combine(MainFolderPath, "video.mp4");
            string FileUrl = "https://github.com/Kinda-Wetty-Today/Minty-Launcher-Releases/releases/download/0.1/BC0B7B0A-C801-4262-85A4-A351E2F99C3A.MP4";
            await DownloadFile(FileUrl, VideoFilePath);
            mediaElement.Source = new Uri(VideoFilePath, UriKind.RelativeOrAbsolute);
            await Task.Delay(48000);
            File.Create(LogFilePath);
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
            return;
        }
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
                MessageBox.Show("Error", $"Error downloading file: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error", $"Error saving file: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
