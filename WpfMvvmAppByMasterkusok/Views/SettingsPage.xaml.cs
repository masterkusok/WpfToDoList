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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    /// 
    public partial class SettingsPage : Page
    {
        public SettingsPage(BaseViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
