using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Foto.WebServer.Shared;

public class BasePage : ComponentBase //, IAsyncDisposable
{
    [Inject] protected NavigationManager? NavigationManager { get; set; }

    // [Inject] protected IJSRuntime? JavaScriptRuntime { get; set; }
    //
    // private IJSObjectReference? _jsObjectReference;
    
    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     await base.OnAfterRenderAsync(firstRender);
    //     // if (firstRender)
    //     // {
    //     //     // Make menu work by importing the JS file when the page is first rendered
    //     //     if (JavaScriptRuntime is not null)
    //     //     {
    //     //         try
    //     //         {
    //     //             _jsObjectReference = await JavaScriptRuntime.InvokeAsync<IJSObjectReference>("import",
    //     //                 "./assets/js/main.js");
    //     //         }
    //     //         catch (JSDisconnectedException)
    //     //         {
    //     //             // Ignore
    //     //             // _logger.LogInformation("Javascript error: {0}", ex.Message);
    //     //         }
    //     //     }
    //     // }
    // }
    // async ValueTask IAsyncDisposable.DisposeAsync()
    // {
    //     // if (_jsObjectReference is not null)
    //     // {
    //     //     try
    //     //     {
    //     //         await _jsObjectReference.DisposeAsync();
    //     //         _jsObjectReference = null;
    //     //     }
    //     //     catch (JSDisconnectedException)
    //     //     {
    //     //     }
    //     // }
    // }
    //
}