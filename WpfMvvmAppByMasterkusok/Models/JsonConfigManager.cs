using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class JsonConfigManager : IConfigManager
    {
        private string _configFileUrl = $"{Directory.GetCurrentDirectory()}/config.json";

        public Configuration Config { get; set; }

        public JsonConfigManager()
        {
            Config = new Configuration();
        }

        public void LoadConfiguration()
        {
            if (File.Exists(_configFileUrl))
            {
                LoadConfigurationFromJson();
                return;
            }
            CreateNewConfigurationFile();
        }

        private void LoadConfigurationFromJson()
        {
            string jsonString = File.ReadAllText(_configFileUrl);
            Config = JsonConvert.DeserializeObject<Configuration>(jsonString);
        }

        private void CreateNewConfigurationFile()
        {
            SetupDefaultConfiguration();
            SaveConfiguration();
        }

        private void SetupDefaultConfiguration()
        {
            Config.CurrentTheme = Theme.LightTheme;
        }

        public void SaveConfiguration()
        {
            File.WriteAllText(_configFileUrl, JsonConvert.SerializeObject(Config));
        }
    }
}
