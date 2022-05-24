using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class PopupRepresenter
    {
        private BaseViewModel _invokingVm;
        private bool _isOpened;
        private string _name;
        public bool IsOpened { get => _isOpened; set => _isOpened = value; }

        public PopupRepresenter(string popupName, BaseViewModel invokingVm)
        {
            _isOpened = false;
            _invokingVm = invokingVm;
            _name = popupName;
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
