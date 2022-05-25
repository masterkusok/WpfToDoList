using System.Windows.Controls;
using System.Windows.Data;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.ViewModels;

namespace WpfMvvmAppByMasterkusok.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(BaseViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
             
        }
    }
}
