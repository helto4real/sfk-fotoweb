using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Foto.WebServer.Shared;

[RenderModeServer]
public class BasePage : ComponentBase //, IAsyncDisposable
{
    [Inject] protected NavigationManager? NavigationManager { get; set; }

    [Inject] protected IJSRuntime? JavaScriptRuntime { get; set; }
}