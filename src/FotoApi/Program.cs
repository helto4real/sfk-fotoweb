using System.Runtime.CompilerServices;
using FotoApi.Common;
using FotoApi.Infrastructure.Repositories.MessagingDbContext;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

[assembly: InternalsVisibleTo("Foto.Tests.Integration")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
var builder = WebApplication.CreateBuilder(args);

// The default username and password for the database is postgres/postgres for local dev only
var connectionString = builder.Configuration.GetConnectionString("FotoApi") ?? "Host=127.0.0.1;Database=PhotoApp;Username=postgres;Password=postgres";
var messagingConnectionString = builder.Configuration.GetConnectionString("Messaging") ?? "Host=127.0.0.1;Database=Messaging;Username=postgres;Password=postgres";
builder.Services.Configure<HostOptions>(o => o.ServicesStartConcurrently = true);
builder.UseFotoApi(connectionString, messagingConnectionString);

var app = builder.Build();

// This is needed for the integration test to work. It will not be a problem in production
await app.Services.GetRequiredService<MessagingDbContext>().Database.EnsureCreatedAsync();
await app.Services.GetRequiredService<PhotoServiceDbContext>().Database.EnsureCreatedAsync();

app.AddFotoApi();

app.Run();
