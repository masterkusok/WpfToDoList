using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Media;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class JsonConfigManager : IConfigManager
    {
        private string _configFileUrl = $"{Directory.GetCurrentDirectory()}/config.json";

        public Theme CurrentTheme { get; set; }

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
            JsonConfigManager storedManager = JsonSerializer.Deserialize<JsonConfigManager>(jsonString);
            CurrentTheme = storedManager.CurrentTheme;
        }

        private void CreateNewConfigurationFile()
        {
            SetupDefaultConfiguration();
            SaveConfiguration();
        }

        private void SetupDefaultConfiguration()
        {
            CurrentTheme = new Theme()
            {
                BGBrush1 = new SolidColorBrush(Color.FromRgb(46, 46, 46)),
                BGBrush2 = new SolidColorBrush(Color.FromRgb(26, 26, 26)),
                FGBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                OPBrush = new SolidColorBrush(Color.FromRgb(80, 101, 217))
            };
        }

        public void SaveConfiguration()
        {
            File.WriteAllText(_configFileUrl, JsonSerializer.Serialize(this));
        }
    }
}
