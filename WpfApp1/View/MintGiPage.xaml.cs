using System;
using System.Diagnostics;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using Button = DiscordRPC.Button;
using Newtonsoft.Json;
using System.IO.Compression;
using static WpfApp1.MainWindow;
using DiscordRPC;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Security.Cryptography;

namespace WpfApp1.View
{
    /// <summary>
    /// Логика взаимодействия для MintGiPage.xaml
    /// </summary>
    public partial class MintGiPage : Page
    {
        private static string key = "hui";
        public MintGiPage()
        {
            InitializeComponent();
        }


        private async void launch_Click(object sender, RoutedEventArgs e)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
            string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
            string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
            string zipFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.zip");
            string verfilePath = System.IO.Path.Combine(assetsFolderPath, "version.txt");
            //string inputFile = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            //string encryptedFile = System.IO.Path.Combine(assetsFolderPath, "encryptedfile.dat");
            //string decryptedFile = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
            string zipUrl = "http://138.2.145.17/minty.zip";
            string vertext = "1.35";
           


            if (File.Exists(verfilePath))
            {
                string verfileContent = File.ReadAllText(verfilePath);
                if (verfileContent.Contains(vertext))
                {
                    if (File.Exists(launcherFilePath))
                    {
                        //DecryptFile(encryptedFile, decryptedFile);
                        this.GI_button.Content = "Launch";
                        LaunchExecutable(launcherFilePath);
                        //EncryptFile(inputFile, encryptedFile);
                        //DiscordRPC();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    File.Delete(verfilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                    File.Delete(zipFilePath);
                    
                }
            }
            else
            {
                this.GI_button.Content = "Downloading";
                Directory.CreateDirectory(assetsFolderPath);
                await DownloadFile(zipUrl, zipFilePath);
                await ExtractZipFile(zipFilePath, assetsFolderPath);
                //EncryptFile(inputFile, encryptedFile);
                File.Delete(zipFilePath);
                this.GI_button.Content = "Launch";
            }
        }





        //metods
        #region
        //SHA256
        #region
        private byte[] GetValidKey()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                return sha256.ComputeHash(keyBytes);
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
        }
        #endregion
        //RPC
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1112360491847778344");
        public static void InitRPC()
        {
            client.Initialize();
        }



        public static void UpdateRPC()
        {
            var presence = new RichPresence()
            {
                State = "Minty",
                Details = "Hacking MHY <333",

                Assets = new Assets()
                {
                    LargeImageKey = "idol",
                    SmallImageKey = "gensh",
                    SmallImageText = "Genshin Impact"
                },
                Buttons = new Button[]
                {
                        new Button()
                        {
                            Label = "Join",
                            Url = "https://discord.gg/kindawindytoday"
                        }
                }
            };
            client.SetPresence(presence);
        }
        public void DiscordRPC()
        {
            InitRPC();
            UpdateRPC();
            for (; ; );
        }
        #endregion
        //EXTRACT
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
                MessageBox.Show("Error while extracting the archive: " + ex.Message);
            }
        }
        #endregion
        //launch
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
        //encrypt
        #region
        private void EncryptFile(string inputFile, string outputFile)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = GetValidKey();
            aesAlg.GenerateIV();

            using FileStream inputFileStream = new FileStream(inputFile, FileMode.Open);
            using FileStream encryptedFileStream = new FileStream(outputFile, FileMode.Create);
            using ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            using CryptoStream cryptoStream = new CryptoStream(encryptedFileStream, encryptor, CryptoStreamMode.Write);

            inputFileStream.CopyTo(cryptoStream);
        }
        #endregion
        //decription
        #region
        private void DecryptFile(string encryptedFile, string decryptedFile)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = GetValidKey();

            using FileStream encryptedFileStream = new FileStream(encryptedFile, FileMode.Open);
            using FileStream decryptedFileStream = new FileStream(decryptedFile, FileMode.Create);
            using ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            using CryptoStream cryptoStream = new CryptoStream(encryptedFileStream, decryptor, CryptoStreamMode.Read);

            cryptoStream.CopyTo(decryptedFileStream);
        }
        #endregion
        #endregion
    }
}

