using Avalonia;
using Avalonia.Logging.Serilog;

namespace NesEmulator.Desktop
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                // Todo: Ensure H/W acceleration works
                .With(new Win32PlatformOptions { AllowEglInitialization = true })
                .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                .LogToDebug();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            app.Run(new MainWindow());
        }
    }
}
