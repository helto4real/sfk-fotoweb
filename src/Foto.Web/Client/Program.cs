using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Foto.Web.Client;
using Foto.Web.Client.ApiClients;
using Foto.Web.Client.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient<UserApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

    // The cookie auth stack detects this header and avoids redirects for unauthenticated
    // requests
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
});

builder.Services.AddHttpClient<AdminApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

    // The cookie auth stack detects this header and avoids redirects for unauthenticated
    // requests
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
});

builder.Services.AddHttpClient<ImagesApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

    // The cookie auth stack detects this header and avoids redirects for unauthenticated
    // requests
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
});
builder.Services.AddHttpClient<StBilderApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

    // The cookie auth stack detects this header and avoids redirects for unauthenticated
    // requests
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
});

builder.Services.AddSingleton<AuthorizationManager>();

await builder.Build().RunAsync();
