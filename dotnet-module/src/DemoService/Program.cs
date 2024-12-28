// Copyright (c) Demo AG. All Rights Reserved.

using DevEpos.CF.Demo.Authentication;
using DevEpos.CF.Demo.Common;
using DevEpos.CF.Demo.Env;
using DevEpos.CF.Demo.ExternalApi;
using DevEpos.CF.Demo.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Serilog.Events;
using Serilog.Templates;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, lc) => {
    var enricher = services.GetRequiredService<LoggerTypeEnricher>();
    if (builder.Environment.IsDevelopment()) {
        lc.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] [id: {correlation_id}] [{logger}] {Message:lj}{NewLine}{Exception}");
    } else {
        lc.WriteTo.Console(new ExpressionTemplate(
            "{ {timestamp: @t, msg: @m, " +
            "level: if @l = 'Information' then 'INFO' else if @l = 'Warning' then 'WARN' else if @l = 'Verbose' then 'TRACE' else @l," +
            " ..@p } }\n"
        ));
    }
    lc.Enrich.With(enricher)
        .Enrich.FromLogContext()
        .MinimumLevel.Information();
});

// Add services to the container.
builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer();

builder.Services.AddAuthorization();

builder.Services.AddControllers(options => {
    options.Filters.Add<HttpResponseExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IKeyManager, KeyManager>();

// configure specifics for dev/non-dev
if (builder.Environment.IsDevelopment()) {
    builder.Services.AddSingleton<IServiceEnv, DummyEnv>();
    builder.Services.ConfigureOptions<DummyJwtBearerOptions>();
} else {
    builder.Services.AddSingleton<IServiceEnv, CfServiceEnv>();
    builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
}

// configure some options
builder.Services.ConfigureOptions<ConfigureAuthorizationOptions>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<SomeProcessor>();
builder.Services.AddSingleton<LoggerTypeEnricher>();

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Hybrid")) {
    // Load Cloud Foundry Environment variables from .env files
    if (!app.Environment.IsDevelopment()) {
        DotEnv.Load(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env")
        );
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction()) {
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) {
    app.MapControllers().AllowAnonymous();
} else {
    app.MapControllers();
}

app.Run();

public partial class Program { }
