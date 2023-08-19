using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        public VideoWindow()
        {
            InitializeComponent();
            video();
        }
        public async void video()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "log");
            string vidFilePath = System.IO.Path.Combine(assetsFolderPath, "video.mp4");
            string logfilePath = System.IO.Path.Combine(assetsFolderPath, "log.txt");
            string logtext = "1";
            string vidUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/video.mp4";
            string textToWrite = "1";
            if (File.Exists(vidFilePath))
            {
                await DownloadFile(vidUrl, vidFilePath);
                    //DisableKeyboardForSeconds(55);
                    //DisableMouseForSeconds(55);
                    PlayVideo(vidFilePath);
                    WriteToFile(logfilePath, textToWrite);
                    await Task.Delay(55000);
                    this.Close();
            }

        }
        private void PlayVideo(string videoPath)
        {
            mediaElement.Source = new Uri(videoPath, UriKind.RelativeOrAbsolute);
            mediaElement.LoadedBehavior = MediaState.Manual; // Устанавливаем ручное управление воспроизведением
            mediaElement.Play();

        }
        private void DisableKeyboardForSeconds(int seconds)
        {
            try
            {
                BlockInput(true); // Отключаем клавиатуру

                Thread.Sleep(seconds * 1000); // Пауза в миллисекундах

                BlockInput(false); // Включаем клавиатуру
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisableMouseForSeconds(int seconds)
        {
            try
            {
                BlockInput(true); // Отключаем мышь

                Thread.Sleep(seconds * 1000); // Пауза в миллисекундах

                BlockInput(false); // Включаем мышь
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        static void WriteToFile(string filePath, string text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
            }
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
                System.Windows.MessageBox.Show($"Error downloading file: {ex.Message}");
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show($"Error saving file: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
