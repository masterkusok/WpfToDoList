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
        private BaseViewModel _currentVM;
        private NavigationStore _navigationStore;
        public BaseViewModel CurrentVM
        {
            get { return _currentVM; }
            set { _currentVM = value; }
        }
        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _currentVM = _navigationStore.CurrentVM;
            NotifyOnPropertyChanged(nameof(CurrentVM));
            CloseWindowCommand = new RelayCommand(obj =>
            {
                Application.Current.MainWindow.Close();
            });
            _navigationStore.CurrentPageChanged += () =>
            {
                CurrentVM = _navigationStore.CurrentVM;
                NotifyOnPropertyChanged(nameof(CurrentVM));
            };
        }

    }
}
