using MudBlazor;

namespace Foto.WebServer.Components;

public class AppAlert : MudAlert
{
    public AppAlert()
    {
        ContentAlignment = HorizontalAlignment.Center;
        Variant = Variant.Text;
    }
    
}