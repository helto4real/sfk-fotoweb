using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using FotoApi;
using FotoApi.Api;
using FotoApi.Common;
using FotoApi.Features.HandleStBilder.Commands;
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
using FotoApi.Infrastructure.Settings;
using FotoApi.Infrastructure.Telemetry;
using FotoApi.Infrastructure.Validation;
using FotoApi.Model;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Oakton.Resources;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

[assembly: InternalsVisibleTo("Foto.Tests.Integration")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
var builder = WebApplication.CreateBuilder(args);

// The default username and password for the database is postgres/postgres for local dev only
var connectionString = builder.Configuration.GetConnectionString("FotoApi") ?? "Host=127.0.0.1;Database=PhotoApp;Username=postgres;Password=postgres";
var messagingConnectionString = builder.Configuration.GetConnectionString("Messaging") ?? "Host=127.0.0.1;Database=Messaging;Username=postgres;Password=postgres";

builder.UseFotoApi(connectionString, messagingConnectionString);

var app = builder.Build();

// This is needed for the integration test to work. It will not be a problem in production
await app.Services.GetRequiredService<MessagingDbContext>().Database.EnsureCreatedAsync();
// await app.Services.GetRequiredService<PhotoServiceDbContext>().Database.EnsureCreatedAsync();

app.AddFotoApi();

app.Run();
