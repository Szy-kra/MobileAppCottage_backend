using FluentValidation;
using FluentValidation.AspNetCore;
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
    // Dodajemy WebApplicationOptions, aby upewniæ siê, ¿e b³êdy ³adowania s¹ lepiej raportowane
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory()
    });

    #region Services Configuration
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Dodajemy obs³ugê JSON, aby unikn¹æ b³êdów przy cyklach w rezerwacjach
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    // --- BAZA DANYCH ---
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<CottageDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)));

    // --- IDENTITY ---
    builder.Services.AddIdentityApiEndpoints<User>()
        .AddRoles<Role>()
        .AddEntityFrameworkStores<CottageDbContext>();

    // --- MEDIATR ---
    // Rejestrujemy na podstawie typu z warstwy Application
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
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    #endregion

    // WY£¥CZAMY restrykcyjne sprawdzanie DI w trybie deweloperskim, 
    // aby aplikacja wsta³a nawet jeœli jakiœ serwis ma problem z zale¿noœci¹.
    builder.Host.UseDefaultServiceProvider((context, options) =>
    {
        options.ValidateScopes = false;
        options.ValidateOnBuild = false;
    });

    var app = builder.Build();

    #region Database Initialization & Seeding
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<CottageDbContext>();
            if (app.Environment.IsDevelopment())
            {
                // U¿ywamy Migrate zamiast EnsureCreated, jeœli masz migracje
                // await dbContext.Database.MigrateAsync(); 
                await dbContext.Database.EnsureCreatedAsync();
            }

            var seeder = services.GetRequiredService<CottageSeeder>();
            await seeder.Seed();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "B³¹d podczas inicjalizacji bazy.");
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

    app.MapGroup("/identity").MapIdentityApi<User>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    #endregion

    logger.Info("Aplikacja ruszy³a!");
    await app.RunAsync();
}
catch (Exception exception)
{
    if (exception.GetType().Name == "HostAbortedException") throw;
    logger.Error(exception, "B³¹d krytyczny startu.");
    throw;
}
finally
{
    LogManager.Shutdown();
}