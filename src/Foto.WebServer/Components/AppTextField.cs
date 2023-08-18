using MudBlazor;

namespace Foto.WebServer.Components;

public class AppTextField<T> : MudTextField<T>
{
    public AppTextField()
    {
        Variant = Variant.Outlined;
        Margin = Margin.Normal;
    }
}