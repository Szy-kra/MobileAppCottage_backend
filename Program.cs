using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.API.Middleware;
using MobileAppCottage.Application.Cottages.Commands.CreateCottage;
using MobileAppCottage.Application.Mappings;
using MobileAppCottage.Application.Services;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;
using MobileAppCottage.Infrastructure.Repositories;
using MobileAppCottage.Infrastructure.UserContext;
using NLog;
using NLog.Web;

// 1. Inicjalizacja NLoga
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory()
    });

    #region Services Configuration
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Obs³uga JSON - ignorowanie cykli (wa¿ne przy relacjach User <-> Cottage)
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    // --- BAZA DANYCH ---
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<CottageDbContext>(options =>
        options.UseSqlServer(connectionString));

    // --- IDENTITY CONFIGURATION ---
    // U¿ywamy AddIdentityApiEndpoints, który generuje tokeny Bearer idealne dla React Native
    builder.Services.AddIdentityApiEndpoints<User>()
        .AddRoles<Role>()
        .AddEntityFrameworkStores<CottageDbContext>()
        .AddDefaultTokenProviders();

    // Blokada b³êdu 500 przy braku serwera poczty
    builder.Services.AddSingleton<IEmailSender<User>, NoOpEmailSender>();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Nie wymagamy potwierdzenia maila
        options.SignIn.RequireConfirmedEmail = false;

        // Konfiguracja hase³ - dopasowana do Twoich testów
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    });

    // --- MEDIATR ---
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(CreateCottageCommand).Assembly));

    // --- WALIDACJA (FluentValidation) ---
    builder.Services.AddValidatorsFromAssembly(typeof(CreateCottageCommand).Assembly);
    builder.Services.AddFluentValidationAutoValidation();

    // --- REJESTRACJA US£UG I REPOZYTORIÓW ---
    builder.Services.AddScoped<ICottageRepository, CottageRepository>();
    builder.Services.AddScoped<CottageSeeder>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<CottageCacheService>();

    // --- AUTOMAPPER ---
    builder.Services.AddAutoMapper(typeof(CottageMappingProfile).Assembly);

    builder.Services.AddScoped<IUserContext, UserContext>();
    builder.Services.AddHttpContextAccessor();

    // --- SWAGGER Z OBS£UG¥ TOKENA ---
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Cottage API", Version = "v1" });
        // Dodajemy mo¿liwoœæ wklejenia tokena w Swaggerze
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Wklej token JWT tutaj",
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        });
    });
    #endregion

    builder.Host.UseDefaultServiceProvider((context, options) =>
    {
        options.ValidateScopes = false;
        options.ValidateOnBuild = false;
    });

    var app = builder.Build();

    #region Database Initialization
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<CottageDbContext>();
            // Database.Migrate() by³oby lepsze, ale EnsureCreated wystarczy do Dockera
            await dbContext.Database.EnsureCreatedAsync();

            var seeder = services.GetRequiredService<CottageSeeder>();
            await seeder.Seed();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "B³¹d inicjalizacji bazy danych.");
        }
    }
    #endregion

    #region Middleware Pipeline
    app.UseMiddleware<ErrorHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Endpointy Identity pod /identity (login, register)
    app.MapGroup("/identity").MapIdentityApi<User>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    #endregion

    logger.Info("Backend CottageApp gotowy do pracy!");
    await app.RunAsync();
}
catch (Exception exception)
{
    if (exception.GetType().Name == "HostAbortedException") throw;
    logger.Error(exception, "Aplikacja zatrzymana przez b³¹d krytyczny.");
    throw;
}
finally
{
    LogManager.Shutdown();
}

// Klasa zapobiegaj¹ca b³êdom 500 przy rejestracji
public class NoOpEmailSender : IEmailSender<User>
{
    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) => Task.CompletedTask;
}