using System;
using System.Windows.Markup;

namespace WpfMvvmAppByMasterkusok.Views
{
    internal class ViewModelToViewExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ViewModeToViewConverter();
        }
    }
}
