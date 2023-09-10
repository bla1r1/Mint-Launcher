//using
#region
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Forms;
using DiscordRPC;
using Button = DiscordRPC.Button;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Threading;
using System.Linq;
using FluentWpfChromes;
#endregion

namespace Minty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskbarIcon taskbarIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();
            DiscordRPC();
        }
        //metods
        #region
        //taskbar
        #region

        private void InitializeTrayIcon()
        {
            taskbarIcon = new TaskbarIcon();
            //taskbarIcon.Icon = new System.Drawing.Icon("L_images/virus.ico");
            taskbarIcon.ToolTipText = "Minty";

            ContextMenu contextMenu = new ContextMenu();

            MenuItem openMenuItem = new MenuItem() { Header = "Open" };
            openMenuItem.Click += OpenMenuItem_Click;
            contextMenu.Items.Add(openMenuItem);

            MenuItem exitMenuItem = new MenuItem() { Header = "Exit" };
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            MenuItem minimizeToTrayMenuItem = new MenuItem() { Header = "Minimize to Tray" };
            minimizeToTrayMenuItem.Click += MinimizeToTrayMenuItem_Click;
            contextMenu.Items.Add(minimizeToTrayMenuItem);

            taskbarIcon.ContextMenu = contextMenu;

            taskbarIcon.TrayLeftMouseDown += TaskbarIcon_LeftMouseDown;

            taskbarIcon.Visibility = Visibility.Visible;
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            taskbarIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        public void MinimizeToTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TaskbarIcon_LeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            taskbarIcon.Dispose();
            base.OnClosed(e);
        }
        public void MinimizeToTray()
        {
            this.Hide();
        }
        #endregion
        //download
        #region
        public async Task DownloadFile(string url, string destinationPath)
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
        #endregion
        //launch
        #region
        public void LaunchExecutable(string exePath)
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
        //RPC
        #region
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1112360491847778344");

        public static void InitRPC()
        {
            client.OnReady += (sender, e) => { };

            client.OnPresenceUpdate += (sender, e) => { };

            client.OnError += (sender, e) => { };

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
            client.Invoke();
        }

        public static void DiscordRPC()
        {
            InitRPC();
            UpdateRPC();
        }


        #endregion
        //dragmove
        #region
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
        //mutex
        #region

        #endregion
        #endregion
    }
}
