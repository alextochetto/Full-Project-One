using IdentityProvider.Data;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using IdentityProvider.Configuration;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Extensions
{
    public static class IdentityConfigurationExtension
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = services.BuildServiceProvider()
                .GetService<IOptionsMonitor<AppSettings>>().CurrentValue;

            if (!appSettings.UseInMemory)
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(nameof(ConnectionStringsOption.DefaultConnection))))
                    .AddIdentity<ApplicationUser, IdentityRole>(config =>
                    {
                        config.SignIn.RequireConfirmedEmail = true;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

                // esta configuração é para o token gerado e enviado para o usuário para recuperar a senha e para confirmar o e-mail do seu cadastro
                // o token é enviado por e-mail para os dois casos
                return services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(1));
            }
            else
            {
                //services.AddRazorPages();
                return services;
            }
        }
    }
}