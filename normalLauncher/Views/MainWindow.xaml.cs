using CommunityToolkit.Mvvm.DependencyInjection;
using Launcher.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;
namespace Launcher.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            WindowManager.Get(this).IsMinimizable = false;
            WindowManager.Get(this).IsMaximizable = false;
            ViewModel = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        }

      

        public NavigationView AppNavigationViewControl { get => AppNavigationView; }

        public Frame ContentFrameControl { get => ContentFrame; }

        public MainWindowViewModel ViewModel { get; }
        }
    }