using DiscordRPC;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace Launcher.View;

public partial class HsrPage : ContentPage
{
	public HsrPage()
	{
		InitializeComponent();
	}
    //Pages
    #region
    private void AboutPage(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AboutPage());
    }
    private void MainPage(object sender, EventArgs e)
    {
        Navigation.PopToRootAsync();
    }

    #endregion
    //launch
    #region
    private async void Launch_Clicked(object sender, EventArgs e)
    {
        var window = Microsoft.Maui.Controls.Application.Current?.MainPage?.Window;
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string mintyFolderPath = Path.Combine(appDataFolder, "minty");
        string assetsFolderPath = Path.Combine(mintyFolderPath, "MintyHSR");
        string launcherFilePath = Path.Combine(assetsFolderPath, "Launcher.exe");
        string dllFilePath = Path.Combine(assetsFolderPath, "minty.dll");
        string zipFilePath = Path.Combine(assetsFolderPath, "minty.zip");
        string verfilePath = Path.Combine(assetsFolderPath, "version.txt");
        string tempFolderPath = System.IO.Path.GetTempPath();
        string updateFilePath = System.IO.Path.Combine(tempFolderPath, "update.exe");
        string serverFileUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/versionHSR.txt";
        string zipUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/mintyHsr.zip";
        string updateUrl = "https://github.com/rusya222/LauncherVer/releases/download/1.0/update.exe";
        string versionUrl = "https://raw.githubusercontent.com/rusya222/LauncherVer/main/LaunchVersion";
        string versionText = await DownloadVersionText(versionUrl);

        if (versionText != null)
        {
            double latestVersion = .0;

            if (!Double.TryParse(versionText, out latestVersion))
            {
                await DisplayAlert("Error", "Unable to parse: " + versionText, "OK");
                return;
            }

            double currentVersion = 1.13;

            if (currentVersion < latestVersion)
            {
                await DisplayAlert("Update", $"launcher got an update.", "OK");
                await DownloadFile(updateUrl, updateFilePath);
                await Task.Delay(2000);
                LaunchExecutable(updateFilePath);
            }
            else
            {
                if (!File.Exists(verfilePath))
                {
                    HSR_button.Text = "Downloading";
                    Directory.CreateDirectory(assetsFolderPath);
                    Directory.CreateDirectory(mintyFolderPath);
                    await DownloadFile(zipUrl, zipFilePath);
                    await ExtractZipFile(zipFilePath, assetsFolderPath);
                    File.Delete(zipFilePath);
                    HSR_button.Text = "Launch";
                    LaunchExecutable(launcherFilePath);
                    //ShowHideWindow();
                }
                else
                {
                    bool filesAreSame = await CheckIfFilesAreSameAsync(serverFileUrl, verfilePath);
                    if (filesAreSame)
                    {
                        if (File.Exists(launcherFilePath))
                        {
                            HSR_button.Text = "Launch";
                            LaunchExecutable(launcherFilePath);
                            //ShowHideWindow();
                        }
                    }
                    else
                    {
                        HSR_button.Text = "Downloading";
                        Directory.CreateDirectory(assetsFolderPath);
                        Directory.CreateDirectory(mintyFolderPath);
                        await DownloadFile(zipUrl, zipFilePath);
                        await ExtractZipFile(zipFilePath, assetsFolderPath);
                        string fileContent = File.ReadAllText(verfilePath);
                        await DisplayAlert("Updated", "Minty updated to version: " + fileContent, "OK");
                        HSR_button.Text = "Launch";
                        //ShowHideWindow();
                    }
                }
            }
        }

    }
    private void LaunchExecutable(string exePath)
    {
        try
        {
            Process.Start(exePath);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error launching executable: {ex.Message}", "OK");
        }
    }
    #endregion
    //download
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
            await DisplayAlert("Error", $"Error downloading file: {ex.Message}", "OK");
        }
        catch (IOException ex)
        {
            await DisplayAlert("Error", $"Error saving file: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    #endregion
    //extract
    #region
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
            await DisplayAlert("Error", "Error while extracting the archive: " + ex.Message, "OK");
        }
    }
    #endregion
    //checkmitver
    #region
    private async Task<bool> CheckIfFilesAreSameAsync(string serverFileUrl, string localFilePath)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string serverFileContent = await client.GetStringAsync(serverFileUrl);
                string localFileContent = await ReadFileAsync(localFilePath);

                return serverFileContent == localFileContent;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error checking files: {ex.Message}", "OK");
            return false;
        }
    }

    private async Task<string> ReadFileAsync(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            return await reader.ReadToEndAsync();
        }
    }
    public async Task<string> DownloadVersionText(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {

                await DisplayAlert("Error", $"Unable to connect to the web server.", "OK");
                return null;
            }

            string versionText = await response.Content.ReadAsStringAsync();
            return versionText;
        }
    }
    #endregion
    //buttons
    #region
    private void Button_Click_Discord(object sender, EventArgs e)
    {
        string link = "https://discord.gg/CpGbZSHKcD";
        Process.Start(new ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        });
    }

    private void Button_Click_Git(object sender, EventArgs e)
    {
        string link = "https://github.com/kindawindytoday/Minty-Old";
        Process.Start(new ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        });
    }

    private void Button_Click_Boosty(object sender, EventArgs e)
    {
        string link = "https://boosty.to/kindawindytoday";
        Process.Start(new ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        });
    }

    private void Button_Click_Youtube(object sender, EventArgs e)
    {
        string link = "https://www.youtube.com/@KindaWindyToday";

        Process.Start(new ProcessStartInfo
        {
            FileName = link,
            UseShellExecute = true
        });
    }
    #endregion
}