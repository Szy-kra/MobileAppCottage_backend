using CottageAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CottageAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Rejestracja bazy danych [cite: 2026-01-12]
            builder.Services.AddDbContext<CottageDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CottageDbConnection")));

            // 2. KONFIGURACJA IDENTITY [cite: 2026-01-12]
            // To spina Twoj¹ klasê User z rolami w bazie danych
            builder.Services.AddIdentityApiEndpoints<User>()
                .AddRoles<IdentityRole>() // Aktywuje obs³ugê AspNetRoles [cite: 2026-01-12]
                .AddEntityFrameworkStores<CottageDbContext>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // 3. MAPOWANIE ENDPOINTÓW LOGOWANIA/REJESTRACJI [cite: 2026-02-03]
            // To stworzy Twoje /identity/login, /identity/register itd.
            app.MapGroup("/identity").MapIdentityApi<User>();

            // 4. AUTHENTICATION & AUTHORIZATION
            // Musz¹ byæ w tej kolejnoœci!
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}