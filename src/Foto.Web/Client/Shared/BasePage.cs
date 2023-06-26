using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Foto.Web.Client.Shared;

public class BasePage : ComponentBase, IAsyncDisposable
{
    [Inject] protected NavigationManager? NavigationManager { get; set; }

    [Inject] protected IJSRuntime? JavaScriptRuntime { get; set; }

    private IJSObjectReference? _jsObjectReference;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // Make menu work by importing the JS file when the page is first rendered
            if (JavaScriptRuntime is not null)
            {
                _jsObjectReference = await JavaScriptRuntime.InvokeAsync<IJSObjectReference>("import",
                    "./assets/js/main.js");
            }
        }
    }
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_jsObjectReference is not null)
        {
            await _jsObjectReference.DisposeAsync();
        }
    }
    
}