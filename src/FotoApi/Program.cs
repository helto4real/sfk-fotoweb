using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using FotoApi;
using FotoApi.Api;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Logging;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authentication.Model;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Settings;
using FotoApi.Infrastructure.Telemetry;
using FotoApi.Infrastructure.Validation;
using FotoApi.Model;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;

[assembly: InternalsVisibleTo("Foto.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
var builder = WebApplication.CreateBuilder(args);

// Add the signalR hub
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("FotoApi") ?? "Data Source=.db/PhotoService.db";
builder.Services.AddSqlite<PhotoServiceDbContext>(connectionString);

builder.Services.AddDbContext<PhotoServiceDbContext>();

builder.AddPhotoApiConfiguration();

// Add Serilog
builder.AddSerilog();

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Configure auth
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();
builder.Services.AddAuthorizationBuilder().AddAdminUserHandler();

// Add the service to generate JWT tokens
builder.Services.AddTokenService();
// Configure the database

// Configure identity
builder.Services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<PhotoServiceDbContext>();
// State that represents the current user from the database *and* the request
builder.Services.AddCurrentUser();

// Configure Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);
// Configure rate limiting
builder.Services.AddRateLimiting();

// Configure initial data for identity database

builder.Services.AddSingleton<HandleExpiredUrlTokensService>();
builder.Services.AddSingleton<IHandleExpiredUrlTokensService, HandleExpiredUrlTokensService>();
builder.Services.AddHostedService<IHandleExpiredUrlTokensService>(s => s.GetRequiredService<IHandleExpiredUrlTokensService>());

builder.Services.AddMailSenderService();
builder.Services.AddScoped<PhotoStore>();
builder.Services.AddScoped<IPhotoStore>(n => n.GetRequiredService<PhotoStore>());
// Configure OpenTelemetry
builder.AddOpenTelemetry();

// add medaiatr
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    // .AddBehavior(typeof(IPipelineBehavior<, >),typeof(ValidationBehaviour<,>)));
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

// add validators
// builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
// add implementation of ExceptionHandling middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Add middleware to handle exceptions and return a JSON response
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add Serilog requests logging
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.Map("/", () => Results.Redirect("/swagger"));

// Configure the APIs
app.MapPhotoImages();
app.MapUsers();
app.MapAdmin();
app.MapPublic();
app.MapStBildApi();

// Configure the prometheus endpoint for scraping metrics
app.MapPrometheusScrapingEndpoint();

app.UseResponseCompression();
app.MapHub<SignalRApi>("/signalr").RequireAuthorization();

app.Run();
