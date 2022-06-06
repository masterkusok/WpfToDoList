using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.ViewModels;

namespace WpfMvvmAppByMasterkusok.Views
{
    /// <summary>
    /// Логика взаимодействия для AccountSettingsView.xaml
    /// </summary>
    public partial class AccountSettingsView : Page
    {
        public AccountSettingsView(BaseViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
