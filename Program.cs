using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.API.Middleware;
using MobileAppCottage.Application.CQRS.Cottages.Commands;
using MobileAppCottage.Application.CQRS.Cottages.Handlers;
using MobileAppCottage.Application.Mappings;
using MobileAppCottage.Application.Services;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;
using MobileAppCottage.Infrastructure.Repositories;
using MobileAppCottage.Infrastructure.Services;
using MobileAppCottage.Infrastructure.UserContext;
using NLog;
using NLog.Web;

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

    // --- CORS (Kluczowe dla React Native / Frontend) ---
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin() // W produkcji zamieñ na konkretne adresy
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

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
    builder.Services.AddIdentityApiEndpoints<User>()
        .AddRoles<Role>()
        .AddEntityFrameworkStores<CottageDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender<User>, NoOpEmailSender>();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    });

    // --- MEDIATR & AUTO-MAPPER (Wszystko z Assembly Application) ---
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateCottageCommandHandler).Assembly));
    builder.Services.AddAutoMapper(typeof(CottageMappingProfile).Assembly);

    // --- WALIDACJA ---
    builder.Services.AddValidatorsFromAssembly(typeof(CreateCottageCommand).Assembly);
    builder.Services.AddFluentValidationAutoValidation();

    // --- REJESTRACJA US£UG I REPOZYTORIÓW ---
    builder.Services.AddScoped<ICottageRepository, CottageRepository>();
    builder.Services.AddScoped<CottageSeeder>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<CottageCacheService>();

    builder.Services.AddScoped<IUserContext, IdentityContext>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IFileService, FileService>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Cottage API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Wklej token JWT tutaj (Bearer {token})",
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

    var app = builder.Build();

    #region Database Initialization
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<CottageDbContext>();
            await dbContext.Database.MigrateAsync();
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

    // Obs³uga plików statycznych (zdjêcia domków)
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // CORS musi byæ przed Authentication/Authorization
    app.UseCors();

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

// No-Op Email Sender dla Identity API
public class NoOpEmailSender : IEmailSender<User>
{
    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) => Task.CompletedTask;
}