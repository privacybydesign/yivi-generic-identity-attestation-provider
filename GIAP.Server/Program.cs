using DotNetEnv;
using GIAP.Server.Configuration;
using GIAP.Server.Middleware;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddTransient<IAttributeMapperService, AttributeMapperService>();
builder.Services.AddTransient<ICredentialAttributeService, CredentialAttributeService>();

builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IIdentityProviderService, IdentityProviderService>(serviceProvider =>
{
    var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    var identityProviderService = new IdentityProviderService(fileSystem);
    // Needs to be initialized immediately after creation because AddIdentityProviders depends on it.
    identityProviderService.Initialize();
    return identityProviderService;
});

builder.Services.AddHttpClient<IApiClient, ApiClient>();
builder.Services.AddHttpClient<ISchemeCredentialClient, SchemeCredentialClient>();
builder.Services.AddHttpClient<IIrmaServerClient, IrmaServerClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services
    .AddAuthentication(configure =>
    {
        configure.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // authenticate using cookies
    })
    .AddCookie()
    .AddIdentityProviders();

// todo temp testing
//https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-8.0&preserve-view=true#forwarded-headers-middleware-order
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedPrefix;

    //https://stackoverflow.com/questions/43749236/net-core-x-forwarded-proto-not-working
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders(); // todo temp testing
app.UseAuthentication(); // todo temp fixing

app.UseMiddleware<IdentityProviderAuthMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapFallbackToFile("/index.html");

app.Run();