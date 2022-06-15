using System;
using System.Windows.Markup;

namespace WpfMvvmAppByMasterkusok.Views
{
    public class PasswordBoxMultipleBindingConverterExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PasswordBoxMultipleBindingConverter();
        }
    }
}
