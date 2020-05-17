using IdentityProvider.Configuration;
using IdentityProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace IdentityProvider.Extensions
{
    public static class IdentityServerStoreExtension
    {
        public static IIdentityServerBuilder AddCustomStore(this IIdentityServerBuilder identityServerBuilder, IConfiguration configuration, IServiceCollection services)
        {
            var appSettings = services.BuildServiceProvider()
                .GetService<IOptionsMonitor<AppSettings>>().CurrentValue;

            if (appSettings.UseInMemory)
            {
                return identityServerBuilder
                    .AddInMemoryIdentityResources(Config.Ids)
                    .AddInMemoryApiResources(Config.Apis)
                    .AddInMemoryClients(Config.Clients);
                    //.AddTestUsers(Config.Users);
            }
            else
            {
                var connectionString = configuration.GetConnectionString(nameof(ConnectionStringsOption.DefaultConnection));
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                return identityServerBuilder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                })
                .AddAspNetIdentity<ApplicationUser>();
            }
        }
    }
}