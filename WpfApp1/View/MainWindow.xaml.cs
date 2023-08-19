using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Button = DiscordRPC.Button;
using DiscordRPC;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Windows.Controls;
using System.Windows.Forms;
using WpfApp1.View;

namespace WpfApp1
{


    public partial class MainWindow : Window
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);


        public MainWindow()
        {
            InitializeComponent();
            video();
            checkversion();
            LoadLauncherInfo();
        }
        

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
                Directory.CreateDirectory(mintyFolderPath);
                Directory.CreateDirectory(assetsFolderPath);
                using (File.Create(logfilePath));
                if (!File.Exists(vidFilePath))
                {
                    await DownloadFile(vidUrl, vidFilePath);
                }
            string verfileContent = File.ReadAllText(logfilePath);
            if (verfileContent.Contains(logtext))
            {

            }
            else
            {
                VideoWindow videoWindow = new VideoWindow();
                videoWindow.Show();
            }
        }


        public async void checkversion()
        {
            string versionUrl = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty1.31.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");
            try
            {
                string versionText = await DownloadVersionText(versionUrl);

                //versionText = versionText.Replace(".", ",");

                if (versionText != null)
                {
                    double latestVersion = .0;

                    if (!Double.TryParse(versionText, out latestVersion))
                    {
                        System.Windows.MessageBox.Show("Unable to parse: " + versionText);
                        return;
                    }

                    double currentVersion = 1.5;

                    if (currentVersion < latestVersion)
                    {
                        File.Delete(verfilePath);
                        File.Delete(launcherFilePath);
                        File.Delete(dllFilePath);
                        string tempFolderPath = System.IO.Path.GetTempPath();
                        string updateFilePath = System.IO.Path.Combine(tempFolderPath, "update.exe");
                        await DownloadFile("https://github.com/rusya222/LauncherVer/releases/download/1.0/update.exe", updateFilePath);
                        await Task.Delay(2000);
                        updateExecutable(updateFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error retrieving launcher version: {ex.Message}");
            }
        }

        private async Task<string> DownloadVersionText(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    System.Windows.MessageBox.Show("Unable to connect to the web server.");
                    return null;
                }

                string versionText = await response.Content.ReadAsStringAsync();
                return versionText;
            }
        }

        public async void LoadLauncherInfo()
        {
            string url = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/Version"; // Replace with the actual URL of your JSON file
            string versString = "1.5";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();

                    LauncherInfo launcherInfo = JsonConvert.DeserializeObject<LauncherInfo>(json);


                    if (launcherInfo != null)
                    {
                        double version = launcherInfo.version;
                        string updateText = launcherInfo.updatetext;
                        string downloadLink1 = launcherInfo.link1;
                        string downloadLink2 = launcherInfo.link2;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }

        public class LauncherInfo
        {
            public double version { get; set; }
            public string updatetext { get; set; }
            public string link1 { get; set; }
            public string link2 { get; set; }
        }

       
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
                System.Windows.MessageBox.Show("Error while extracting the archive: " + ex.Message);
            }
        }
        private async Task UpdateLauncher(string url, string destinationPath)
        {
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(destinationPath));

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }

                    File.Move(tempFilePath, destinationPath);
                    System.Windows.MessageBox.Show("File downloaded successfully!");
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



        private void updateExecutable(string exePath)
        {
            try
            {
                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error launching executable: {ex.Message}");
            }
        }


        #endregion

        //Buttons
        #region  
        private void Button_Click_Discord(object sender, RoutedEventArgs e)
        {
            string link = "https://discord.gg/CpGbZSHKcD";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });
        }

        private void Button_Click_Git(object sender, RoutedEventArgs e)
        {

            string link = "https://github.com/kindawindytoday/Minty-Old";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Boosty(object sender, RoutedEventArgs e)
        {

            string link = "https://boosty.to/kindawindytoday";

            
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Tiktok(object sender, RoutedEventArgs e)
        {

            string link = "https://www.tiktok.com/@kindawindytoday?ug_source=op.auth&ug_term=Linktr.ee&utm_source=awyc6vc625ejxp86&utm_campaign=tt4d_profile_link&_r=1";

            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }
        private void Button_Click_Youtube(object sender, RoutedEventArgs e)
        {

            string link = "https://www.youtube.com/@KindaWindyToday";
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true
            });



        }

        #endregion

        

        
        async Task<string> HttpResponse(string line)
        {
            using (var net = new HttpClient())
            {
                var responce = await net.GetAsync(line);
                return responce.IsSuccessStatusCode ? await responce.Content.ReadAsStringAsync() : null;
            }
        }


    }
}
