using WpfMvvmAppByMasterkusok.Commands;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.Stores;
using System.Windows.Input;
using System.Windows;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class MainWindowViewModel:BaseViewModel
    {
        public ICommand CloseWindowCommand
        {
            get;
        }
        private Page _currentPage;
        private NavigationStore _navigationStore;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            CurrentPage = _navigationStore.CurrentPage;
            NotifyOnPropertyChanged(nameof(CurrentPage));
            CloseWindowCommand = new RelayCommand(obj =>
            {
                Application.Current.MainWindow.Close();
            });
            _navigationStore.CurrentPageChanged += () =>
            {
                CurrentPage = _navigationStore.CurrentPage;
                NotifyOnPropertyChanged(nameof(CurrentPage));
            };
        }

    }
}
