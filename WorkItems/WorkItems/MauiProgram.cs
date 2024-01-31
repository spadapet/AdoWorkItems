using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Xceed.Maui.Toolkit;

namespace WorkItems;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp.CreateBuilder()
            .UseXceedMauiToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(MauiProgram.ConfigureFonts)
            .Build();
    }

    private static void ConfigureFonts(IFontCollection fonts)
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    }
}
