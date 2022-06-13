using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class PopupRepresenter
    {
        private BaseViewModel _invokingVm;
        private bool _isOpened;
        private string _name;

        public bool IsOpened { get => _isOpened; set => _isOpened = value; }
        public ICommand OpenCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public PopupRepresenter(string notifyingPropertyName, BaseViewModel invokingVm)
        {
            _isOpened = false;
            _invokingVm = invokingVm;
            _name = notifyingPropertyName;

            OpenCommand = new RelayCommand(obj =>
            {
                Open();
            });

            CloseCommand = new RelayCommand(obj =>
            {
                Close();
            });
        }

        public async void ShowWithTimer(int milliseconds)
        {
            Open();
            await Task.Delay(milliseconds);
            Close();
        }

        public void Open()
        {
            _isOpened = true;
            _invokingVm.NotifyOnPropertyChanged($"{_name}");
        }

        public void Close()
        {
            _isOpened = false;
            _invokingVm.NotifyOnPropertyChanged($"{_name}");
        }

    }
}
