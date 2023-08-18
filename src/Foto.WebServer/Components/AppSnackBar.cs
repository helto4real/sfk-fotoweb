using MudBlazor;

namespace Foto.WebServer.Components;

public class AppSnackBar(ISnackbar snackbar)
{
    public void Alert(string message, Severity severity)
    {
        snackbar.Add(message, severity, c =>
        {
            c.VisibleStateDuration = 5000;
            c.SnackbarVariant = Variant.Filled;
            c.ShowCloseIcon = true;
        });
    }
}