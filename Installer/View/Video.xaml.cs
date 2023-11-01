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
            string fileUrl = "https://github.com/Kinda-Wetty-Today/Minty-Launcher-Releases/releases/download/0.1/BC0B7B0A-C801-4262-85A4-A351E2F99C3A.MP4";
            string videofile = "video.mp4";
            using (HttpClient client = new HttpClient())
            {
                    HttpResponseMessage response = await client.GetAsync(fileUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(videofile, fileBytes);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при скачивании файла. Код состояния: " + response.StatusCode);
                    }
            }
            mediaElement.Source = new Uri(videofile, UriKind.RelativeOrAbsolute);
            await Task.Delay(48000);
            File.Create(LogFilePath);
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
            return;
        }
    }
}
