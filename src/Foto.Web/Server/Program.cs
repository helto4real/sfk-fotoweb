using Foto.Web.Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure auth with the front end
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddPolicy("AdminPolicy", policy =>
{
    policy.RequireRole("Admin");
});

// Add razor pages so we can render the Blazor WASM todo component
builder.Services.AddRazorPages();
    
// Add the forwarder to make sending requests to the backend easier
builder.Services.AddHttpForwarder();
var photoApiUrl = builder.Configuration.GetServiceUri("fotoapp")?.ToString() ??
              builder.Configuration["FotoApiUrl"] ?? 
              throw new InvalidOperationException("Todo API URL is not configured");

// Configure the HttpClient for the backend API
builder.Services.AddHttpClient<Foto.Web.Server.FotoApi>(client =>
{
    client.BaseAddress = new(photoApiUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToPage("/_Host");

// Configure the APIs
app.MapAuth();
// app.MapAdmin(photoApiUrl);
// app.MapPublic(photoApiUrl);
app.MapUser();
app.MapPhotoImages(photoApiUrl);

app.Run();

