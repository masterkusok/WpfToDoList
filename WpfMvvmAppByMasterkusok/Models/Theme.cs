using System;
using System.Windows.Media;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class Theme
    {
        public static readonly Theme DarkTheme = new Theme()
        {
            BGBrush1 = new SolidColorBrush(Color.FromRgb(46, 46, 46)),
            BGBrush2 = new SolidColorBrush(Color.FromRgb(26, 26, 26)),
            FGBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            OPBrush = new SolidColorBrush(Color.FromRgb(80, 101, 217))
        };

        public static readonly Theme LightTheme = new Theme()
        {
            BGBrush1 = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
            BGBrush2 = new SolidColorBrush(Color.FromRgb(168, 168, 168)),
            FGBrush = new SolidColorBrush(Color.FromRgb(10, 10, 10)),
            OPBrush = new SolidColorBrush(Color.FromRgb(235, 45, 45))
        };

        public SolidColorBrush BGBrush1 { get; set; }
        public SolidColorBrush BGBrush2 { get; set; }
        public SolidColorBrush FGBrush { get; set; }
        public SolidColorBrush OPBrush { get; set; }
    }
}
