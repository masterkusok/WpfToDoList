namespace WpfMvvmAppByMasterkusok.Models
{
    public interface IConfigManager
    {
        public Theme CurrentTheme { get; }
        public void LoadConfiguration();
        public void SaveConfiguration();
    }
}
