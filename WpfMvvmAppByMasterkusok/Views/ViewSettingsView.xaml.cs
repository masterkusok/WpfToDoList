using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfMvvmAppByMasterkusok.ViewModels;

namespace WpfMvvmAppByMasterkusok.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewSettingsView.xaml
    /// </summary>
    public partial class ViewSettingsView : Page
    {
        public ViewSettingsView(BaseViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
