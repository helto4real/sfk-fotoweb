using System.Net;
using Foto.WebServer.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Foto.WebServer.Services;

public abstract class ServiceBase(ILogger logger)
{
    [Inject] public JSRuntime? JsRuntime { get; set; }
    
    protected async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return null;
        try
        {
            if (response.Content.Headers is { ContentLength: > 0, ContentType.MediaType: "application/json" })
                return await response.Content.ReadFromJsonAsync<ErrorDetail>();
                
            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
            {
                logger.LogInformation("User is not authorized to access {Path}", response.RequestMessage?.RequestUri);
                return new ErrorDetail { Title = "Åtkomst nekad", Detail = "Du har inte behörighet att utföra denna åtgärd.", StatusCode = (int) response.StatusCode};
            }
                
            return new ErrorDetail { Title = "Systemfel", Detail = "Något gick fel att hantera felmeddelande, kontakta administratören.", StatusCode = (int) response.StatusCode};
        }
        catch
        {
            // We have another result than ErrorDeatil, so we can't read it as ErrorDetail
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError($"Server returned an unknown error: {error}");
            return new ErrorDetail { Title = "Systemfel", Detail = "Något gick fel att hantera felmeddelande, kontakta administratören." };
        }
    }
}