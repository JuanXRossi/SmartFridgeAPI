using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartFridgeAPI.Exceptions;
using SmartFridgeAPI.Interfaces;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Models.Data;
using SmartFridgeAPI.Repository;
using SmartFridgeAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File(
        "logs/smartfridge-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 100000;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme = 
    options.DefaultForbidScheme = 
    options.DefaultScheme =
    options.DefaultSignInScheme = 
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigninKey"])
        ),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var reason = context.AuthenticateFailure?.GetType().Name switch
            {
                "SecurityTokenExpiredException" => "El token ha expirado.",
                "SecurityTokenInvalidSignatureException" => "Token inválido.",
                _ => "No autorizado"
            };

            var body = JsonSerializer.Serialize(new
            {
                success = false,
                message = reason
            });

            await context.Response.WriteAsync(body);
        }
    };
});

builder.Services.AddValidation();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUrgencyRepository, UrgencyRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    await context.Database.CanConnectAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
        {
            return Serilog.Events.LogEventLevel.Error;
        }
        if (httpContext.Response.StatusCode >= 400)
        {
            return Serilog.Events.LogEventLevel.Warning;
        }

        return Serilog.Events.LogEventLevel.Information;
    };

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "Anonymous");
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
    };
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
