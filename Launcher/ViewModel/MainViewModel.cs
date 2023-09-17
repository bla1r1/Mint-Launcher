using GalaSoft.MvvmLight.Command;
using Minty.View;
using System.Windows.Controls;
using System.Windows.Input;
namespace Minty.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {

        private Page mgi = new MintGiPage();
        private Page mhsr = new MintHsrPage();
        private Page ab = new AboutPage();
        private Page s = new SettingsPage();
        private Page _CurPage = new MintGiPage();


        public Page CurPage
        {
            get => _CurPage;
            set => Set(ref _CurPage, value);
        }

        public ICommand OpenMintGiPage
        {
            get
            {
                return new RelayCommand(() => CurPage = mgi);
            }
        }

        public ICommand OpenMintHsrPage
        {
            get
            {
                return new RelayCommand(() => CurPage = mhsr);
            }
        }

        public ICommand OpenAboutPage
        {
            get
            {
                return new RelayCommand(() => CurPage = ab);
            }
        }
        public ICommand OpenSettinsPage
        {
            get
            {
                return new RelayCommand(() => CurPage = s);
            }
        }
    }
}
