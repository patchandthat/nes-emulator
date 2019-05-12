using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NesEmulator.Desktop
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _surface = this.FindControl<RenderSurface>("Surface");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private RenderSurface _surface;

        private void nextFrame(object sender, EventArgs e)
        {
            var frame = _surface.BeginFrame();
            _surface.Draw(frame);
        }
    }
}
