using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using BlazorStrap;
using Foto.WebServer.Api;
using Foto.WebServer.Authentication;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var appSettingSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingSection);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.ConsentCookie.Name = "ConsentCookie";
    options.CheckConsentNeeded = context => true;
});
// Configure auth with the front end
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddPolicy("AdminPolicy", policy =>
{
    policy.RequireRole("Admin");
});

builder.Services.AddRazorPages();
builder.Services.AddHttpForwarder();
builder.Services.AddServerSideBlazor();
var photoApiUrl = builder.Configuration.GetServiceUri("fotoapp")?.ToString() ??
                  builder.Configuration["AppSettings:FotoApiUrl"] ?? 
                  throw new InvalidOperationException("Todo API URL is not configured");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpContextAccessor();

// Add the forwarder to make sending requests to the backend easier

// Configure the HttpClient for the backend API

// Configure the HttpClient for the backend API
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddHttpClient<IUserService, UserService>(client => client.BaseAddress = new(photoApiUrl));
builder.Services.AddHttpClient<IAdminService, AdminService>(client => client.BaseAddress = new(photoApiUrl));
builder.Services.AddHttpClient<IStBildService, StBildService>(client => client.BaseAddress = new(photoApiUrl));
builder.Services.AddHttpClient<IImageService, ImageService>(client => client.BaseAddress = new(photoApiUrl));

builder.Services.AddBlazorStrap();

builder.Services.AddScoped<TokenAuthorizationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthorizationProvider>());
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
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapAuth();
app.MapCookieConsent();
app.MapPhotoImages(photoApiUrl);
// app.MapUser();
app.UseCookiePolicy();
app.Run();