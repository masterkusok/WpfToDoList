using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.ViewModels;

namespace WpfMvvmAppByMasterkusok.Views
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage(BaseViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
