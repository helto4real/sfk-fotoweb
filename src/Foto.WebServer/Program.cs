using System.Text.Json.Serialization;
using Foto.WebServer.Api;
using Foto.WebServer.Authentication;
using Foto.WebServer.Components;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Foto.WebServer.Shared.Logging;
using Microsoft.AspNetCore.Http.Json;
using MudBlazor;
using MudBlazor.Services;
using Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var appSettingSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingSection);
builder.Services.Configure<HostOptions>(o => o.ServicesStartConcurrently = true);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.ConsentCookie.Name = "ConsentCookie";
    options.CheckConsentNeeded = _ => true;
});

builder.Services.AddMemoryCache();

// Configure auth with the front end
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddPolicy("AdminPolicy", policy =>
{
    policy.RequireRole("Admin");
});

builder.AddAuthorizationPolicies();

builder.Services.AddRazorPages();
//builder.Services.AddRazorComponents().AddServerComponents();
builder.Services.AddHttpForwarder();
// The reverse proxy for SignalR
var proxySettings = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy()
    .LoadFromConfig(proxySettings);

builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(c =>
{
    c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

builder.Services.AddScoped<AppSnackBar>();

builder.Services.AddScoped<MudThemeProvider>();

var photoApiUrl = builder.Configuration.GetServiceUri("fotoapp")?.ToString() ??
                  builder.Configuration["AppSettings:FotoApiUrl"] ?? 
                  throw new InvalidOperationException("Todo API URL is not configured");

builder.Services.AddHttpContextAccessor();

// Configure the HttpClient for the backend API
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddScoped<AuthorizedHttpClientHandler>();
// We cannot use the handler in auth service since it is used in the handler
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<ISignInService, SignInService>();

builder.Services.AddHttpClient<IUserService, UserService>()
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizedHttpClientHandler>());
builder.Services.AddHttpClient<IAdminService, AdminService>()
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizedHttpClientHandler>());
builder.Services.AddHttpClient<IMemberService, MemberService>()
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizedHttpClientHandler>());
builder.Services.AddHttpClient<IStBildService, StBildService>()
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizedHttpClientHandler>());
builder.Services.AddHttpClient<IImageService, ImageService>()
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizedHttpClientHandler>());

// Register notification service
builder.Services.AddScoped(typeof(INotificationService<>), typeof(NotificationService<>));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<RefreshTokenMiddleware>();
app.UseAuthorization();
app.UseAntiforgery();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
// app.MapRazorComponents<App>();
//app.MapRazorPages();
app.MapAuthenticationApi();
app.MapCookieConsentApi();
app.MapImageForwardApi(photoApiUrl);
app.MapDownloadForwardApi(photoApiUrl);
app.MapReverseProxy();
// app.MapUser();
app.UseCookiePolicy();
app.Run();
