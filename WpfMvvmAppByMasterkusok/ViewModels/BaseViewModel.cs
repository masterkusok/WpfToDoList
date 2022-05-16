using System.ComponentModel;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.Stores;
namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        protected NavigationStore _navigationStore;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyOnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void MoveToPage(Page page)
        {
            if (_navigationStore != null)
            {
                _navigationStore.CurrentPage = page;
            }
        }
    }
}
