using Avalonia;
using Avalonia.Markup.Xaml;

namespace NesEmulator.Desktop
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
