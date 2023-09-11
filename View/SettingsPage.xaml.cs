using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace Minty.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        
        public SettingsPage()
        {
            InitializeComponent();
            
        }
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
                    string verFilePath = System.IO.Path.Combine(assetsFolderPath, "ver.txt");
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
                // Обработка ошибок чтения файла, например, вывод сообщения об ошибке или логирование.
                MessageBox.Show("Произошла ошибка при чтении файла: " + ex.Message);
            }
        }
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
                    string verFilePath = System.IO.Path.Combine(assetsFolderPath, "ver.txt");
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
                // Обработка ошибок чтения файла, например, вывод сообщения об ошибке или логирование.
                MessageBox.Show("Произошла ошибка при чтении файла: " + ex.Message);
            }
        }
           
        }
    }

