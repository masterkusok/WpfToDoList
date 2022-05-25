using System.ComponentModel;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.Stores;
namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected NavigationStore _navigationStore;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyOnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void ChangeCurrentVM(BaseViewModel vm)
        {
            if (_navigationStore != null)
            {
                _navigationStore.CurrentVM = vm;
            }
        }
        public NavigationStore GetNavigationStore()
        {
            return _navigationStore;
        }
    }
}
