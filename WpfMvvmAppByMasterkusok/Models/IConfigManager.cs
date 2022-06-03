namespace WpfMvvmAppByMasterkusok.Models
{
    public interface IConfigManager
    {
        public Configuration Config { get; }
        public void LoadConfiguration();
        public void SaveConfiguration();
    }
}
