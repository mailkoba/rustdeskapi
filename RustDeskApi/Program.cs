using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RustDeskApi;
using RustDeskApi.Filters;
using RustDeskApi.Services;
using RustDeskApi.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
       .UseSerilog((context, services, configuration) =>
                       configuration.ReadFrom.Configuration(context.Configuration)
                                    .ReadFrom.Services(services)
                                    .Enrich.FromLogContext(),
                   writeToProviders: true)
       .UseDefaultServiceProvider((context, options) => options.ValidateScopes = true);

// Add services to the container.
builder.Services.AddSingleton(typeof (Microsoft.Extensions.Logging.ILogger),
                              provider => provider.GetRequiredService<ILoggerFactory>()
                                                  .CreateLogger("RustDeskApi"));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizeFilter>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ApiSettings>(builder.Configuration);
builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IScopeProvider, ScopeProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = ApplicationConstants.Jwt.Issuer,
                ValidAudience = ApplicationConstants.Jwt.Audience,
                ValidateLifetime = false,
                IssuerSigningKey = ApplicationConstants.Jwt.SigningKey,
                ValidateIssuerSigningKey = true
            };
        });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseMiddleware<GetAccountMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
