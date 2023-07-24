using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Foto.WebServer.Services;

public class ServiceBase
{
    [Inject] public JSRuntime? JsRuntime { get; set; }
}