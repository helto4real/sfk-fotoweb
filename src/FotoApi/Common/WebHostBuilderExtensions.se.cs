using System.Text.Json.Serialization;
using FluentValidation;
using FotoApi.Api;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Features.HandleUrlTokens;
using FotoApi.Features.SendEmailNotifications;
using FotoApi.Infrastructure.Api;
using FotoApi.Infrastructure.ExceptionsHandling;
using FotoApi.Infrastructure.Logging;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.MessagingDbContext;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authentication;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Infrastructure.Security.Authorization.Policies;
using FotoApi.Infrastructure.Settings;
using FotoApi.Infrastructure.Telemetry;
using FotoApi.Model;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Oakton.Resources;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

namespace FotoApi.Common;

internal static class WebHostBuilderExtensions_se
{
    public static WebApplicationBuilder UseFotoApi(this WebApplicationBuilder builder, string connectionString, string messagingConnectionString)
    {
        builder
            .AddPhotoApiConfiguration()
            .UseSerilogLogging()
            .AddAspNetConfig()
            .AddSignalRServices()
            .UseWolverine(messagingConnectionString)
            .AddFotoApiServices()
            .AddPhotoApiDb(connectionString);
        
        return builder;
    }

    public static WebApplication AddFotoApi(this WebApplication app)
    {
        // Add middleware to handle exceptions and return a JSON response
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        // Add Serilog requests logging
        app.UseSerilogRequestLogging();
        app.AddSwagger();
        app.UseRateLimiter();
        app.AddEndpoints();
        app.UseResponseCompression();
        app.AddSignalR();
        return app;
    }

    private static WebApplication AddEndpoints(this WebApplication app)
    {
        // Configure the APIs
        app.MapPhotoImages();
        app.MapUsers();
        app.MapAdmin();
        app.MapPublic();
        app.MapStBildApi();
        app.MapMemberApi();

        // Configure the prometheus endpoint for scraping metrics
        app.MapPrometheusScrapingEndpoint();
        return app;
    }
    private static HubEndpointConventionBuilder AddSignalR(this WebApplication app) => app.MapHub<SignalRApi>("/signalr").RequireAuthorization();
    private static WebApplication AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.Map("/", () => Results.Redirect("/swagger"));
        return app;
    }

    private static WebApplicationBuilder AddPhotoApiDb(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddNpgsql<PhotoServiceDbContext>(connectionString);
        builder.Services.AddDbContext<PhotoServiceDbContext>();
        return builder;
    }
    private static WebApplicationBuilder AddFotoApiServices(this WebApplicationBuilder builder)
    {
        // Add the service to generate JWT tokens
        builder.Services.AddTokenService();
        builder.Services.AddSingleton<HandleExpiredUrlTokensService>();
        builder.Services.AddSingleton<IHandleExpiredUrlTokensService, HandleExpiredUrlTokensService>();
        builder.Services.AddHostedService<IHandleExpiredUrlTokensService>(s => s.GetRequiredService<IHandleExpiredUrlTokensService>());

        builder.Services.AddMailSenderService();
        builder.Services.AddScoped<PhotoStore>();
        builder.Services.AddScoped<IPhotoStore>(n => n.GetRequiredService<PhotoStore>());
        
        builder.Services.AddFotoAppHandlers();
        builder.Services.AddFotoAppPipelines();
        
        // add validators
        builder.Services.AddValidatorsFromAssemblyContaining<NewStBildRequestValidator>();
        // add implementation of ExceptionHandling middleware
        builder.Services.AddTransient<ExceptionHandlingMiddleware>();
        return builder;
    }

    private static WebApplicationBuilder UseSerilogLogging(this WebApplicationBuilder builder) => builder.AddSerilog();

    private static WebApplicationBuilder AddAspNetConfig(this WebApplicationBuilder builder)
    {
        // Configure auth
        builder.AddAuthentication();
        builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();
        builder.Services.AddAuthorizationBuilder().AddAdminUserHandler();
        builder.AddAuthorizationPolicies();
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });
        builder.Services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
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
        
        // Configure OpenTelemetry
        builder.AddOpenTelemetry();
        return builder;
    }
    
    private static WebApplicationBuilder AddSignalRServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR(o =>
        {
            o.AddFilter<AuthHubFilter>();
        });
        return builder;
    }
    
    private static WebApplicationBuilder UseWolverine(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Host.UseWolverine(opts =>
        {
            opts.PersistMessagesWithPostgresql(connectionString);
            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.Policies.AutoApplyTransactions();
        });
        builder.Host.UseResourceSetupOnStartup();
        builder.Services.AddNpgsql<MessagingDbContext>(connectionString);
        builder.Services.AddDbContext<MessagingDbContext>();
        return builder;
    }
}