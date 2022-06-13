using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class PopupRepository
    {
        public ObservableCollection<PopupRepresenter> PagePopups = new ObservableCollection<PopupRepresenter>();
        private Dictionary<string, int> _keyIndexPairs = new Dictionary<string, int>();
        private string _propertyName;
        public PopupRepository(string propertyName)
        {
            _propertyName = propertyName;
        }
        public void AddPopupWithKey(string key, PopupRepresenter popup)
        {
            if (!_keyIndexPairs.ContainsKey(key))
            {
                PagePopups.Add(popup);
                _keyIndexPairs.Add(key, PagePopups.Count-1);
                return;
            }
            throw new Exception("Key already exists");
        }
    }
}
