using GIAP.Server.Configuration;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddTransient<AttributeMapperService>();
builder.Services.AddTransient<CredentialAttributeService>();

builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IdentityProviderService>(serviceProvider =>
{
    var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    var identityProviderService = new IdentityProviderService(fileSystem);
    // Needs to be initialized immediately after creation because AddIdentityProviders depends on it.
    identityProviderService.Initialize();
    return identityProviderService;
});

builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddHttpClient<SchemeCredentialClient>();
builder.Services.AddHttpClient<IrmaServerClient>();

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

var app = builder.Build();

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