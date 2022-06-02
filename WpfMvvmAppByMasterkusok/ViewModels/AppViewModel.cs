using WpfMvvmAppByMasterkusok.Models;
using System.Media;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class AppViewModel
    {
        private IConfigManager _configManager;

        public Theme CurrentTheme {  get => _configManager.CurrentTheme; }

        public AppViewModel()
        {
            _configManager = new JsonConfigManager();
            _configManager.LoadConfiguration();
        }
    }
}
