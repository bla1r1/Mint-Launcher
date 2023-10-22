namespace Launcher.Services
{
    internal class DownloadService
    {
        public XamlRoot XamlRoot { get; private set; }

        public static async Task<bool> DownloadFilesAsync(string downloadUrl, string verUrl, string zipFilePath, string verFilePath, string assetsFolderPath, string launcherFilePath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    async Task WriteDownloadedBytesToDisk(byte[] content, string filePath)
                    {
                        await File.WriteAllBytesAsync(filePath, content);
                    }
                    using (var downloadTask = httpClient.GetByteArrayAsync(downloadUrl))
                    {
                        using (var verTask = httpClient.GetByteArrayAsync(verUrl))
                        {
                            var tasks = new[] { downloadTask, verTask };
                            await Task.WhenAll(tasks);
                            foreach (var task in tasks)
                            {
                                task.Dispose();
                            }
                            await WriteDownloadedBytesToDisk(downloadTask.Result, zipFilePath);
                            await WriteDownloadedBytesToDisk(verTask.Result, verFilePath);
                        }
                    }
                }

                await ExtractZipFile(zipFilePath, assetsFolderPath);
                File.Delete(zipFilePath);
                return true;
            }
            catch (HttpRequestException ex)
            {
                ShellPage.ShowErrorDialog($"Error downloading file: {ex.Message}");
            }
            catch (IOException ex)
            {
                ShellPage.ShowErrorDialog($"Error saving file: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShellPage.ShowErrorDialog($"An unexpected error occurred: {ex.Message}");
            }

            return false;
        }

        private static async Task ExtractZipFile(string zipFilePath, string extractionPath)
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
                ShellPage.ShowErrorDialog($"Error while extracting the archive: {ex.Message}");
            }
        } 
    }

}
