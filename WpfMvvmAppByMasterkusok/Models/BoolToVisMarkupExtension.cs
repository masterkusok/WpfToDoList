using System;
using System.Windows.Markup;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class BoolToVisMarkupExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new InvertableBoolToVisConverter();
        }
    }
}
