using MudBlazor;
using MudBlazor.Utilities;

namespace Foto.WebServer.Shared;

public static class CustomTheme
{
    public static MudTheme Theme { get; } = new()
    {
        PaletteDark = new PaletteDark
        {
            AppbarBackground = new MudColor("#1c1c21")
        },
        Palette = new PaletteLight
        {
            AppbarBackground = new MudColor("#3463ae"),
            AppbarText = new MudColor(Colors.Shades.White),
            Primary = new MudColor("#3463ae")
        },
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = new[] { "Poppins", "Helvetica", "Arial", "sans-serif" }
            }
        }
    };

    
}