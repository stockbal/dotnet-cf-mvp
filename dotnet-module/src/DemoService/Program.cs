using DevEpos.CF.Demo.Authentication;
using DevEpos.CF.Demo.Common;
using DevEpos.CF.Demo.Env;
using DevEpos.CF.Demo.ExternalApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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

if (app.Environment.IsDevelopment()) {
    app.MapControllers().AllowAnonymous();
} else {
    app.MapControllers();
}

app.Run();

public partial class Program { }
