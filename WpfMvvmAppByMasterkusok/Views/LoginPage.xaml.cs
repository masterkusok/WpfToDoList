using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.ViewModels;

namespace WpfMvvmAppByMasterkusok.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage(NavigationStore navigationStore)
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(navigationStore);
        }
    }
}
