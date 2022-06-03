using WpfMvvmAppByMasterkusok.Models;
using System.Media;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class AppViewModel
    {
        private IConfigManager _configManager;

        public Configuration Config {  get => _configManager.Config; }

        public AppViewModel()
        {
            _configManager = new JsonConfigManager();
            _configManager.LoadConfiguration();
        }
    }
}
