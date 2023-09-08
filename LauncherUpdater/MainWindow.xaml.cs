using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;

namespace LauncherUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int currentProgress;
        public MainWindow()
        {
            InitializeComponent();
            Timer();
        }
        //Metods
        #region
        //Progressbar
        #region
        private async Task Timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            currentProgress = 0;
            ProgressBar.Visibility = Visibility.Hidden;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value = currentProgress;
        }
        #endregion
        //Close
        #region
        private void Close(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion
        //Update
        #region
        private async void Update(object sender, RoutedEventArgs e)
        {
            string updateFilePath = "Update.zip";
            string updateFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string mintFilePath = "Minty.exe";
            string updateUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/Update.zip";

            cl.Visibility = Visibility.Hidden; 
            up.Visibility = Visibility.Hidden;
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            timer.Start();
            await DownloadFile(updateUrl, updateFilePath);
            await ExtractZipFile(updateFilePath, updateFolderPath);
            File.Delete(updateFilePath);
            ProgressBar.Visibility = Visibility.Hidden;
            cl.Visibility = Visibility.Visible;
            up.Visibility = Visibility.Visible;
            LaunchExecutable(mintFilePath);
        }
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
        //Download + Extract
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
        #endregion
        #endregion
    }
}
