//usings
#region
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
#endregion
namespace Minty.View
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        //Metods
        #region
        //DeleteGI
        #region
        private void DeleteGI(object sender, RoutedEventArgs e)
        {
            try
            {

                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure ?", "Delete MintyGI", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
                    string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyGI");
                    string verFilePath = System.IO.Path.Combine(assetsFolderPath, "verGI.txt");
                    string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
                    string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
                    File.Delete(verFilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                }
                else if (result == DialogResult.No)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при чтении файла: " + ex.Message);
            }
        }
        #endregion
        //DeleteSR
        #region
        private void DeleteSR(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure ?", "Delete MintySR", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string mintyFolderPath = System.IO.Path.Combine(appDataFolder, "minty");
                    string assetsFolderPath = System.IO.Path.Combine(mintyFolderPath, "MintyHSR");
                    string launcherFilePath = System.IO.Path.Combine(assetsFolderPath, "Launcher.exe");
                    string dllFilePath = System.IO.Path.Combine(assetsFolderPath, "minty.dll");
                    string verFilePath = System.IO.Path.Combine(assetsFolderPath, "verSR.txt");
                    File.Delete(verFilePath);
                    File.Delete(launcherFilePath);
                    File.Delete(dllFilePath);
                }
                else if (result == DialogResult.No)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при чтении файла: " + ex.Message);
            }
        }
        #endregion
        #endregion
    }
}

