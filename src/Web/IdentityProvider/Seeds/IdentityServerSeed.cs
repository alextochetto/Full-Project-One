using IdentityProvider.Data;
using IdentityProvider.Models;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace IdentityProvider.Seeds
{
    public static class IdentityServerSeed
    {
        public static void RunMigration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        }

        public static void SeeClient(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            #region Payment Client
            if (!context.Clients.Any(_ => _.ClientId == "payment-client"))
            {
                var client = new Client
                {
                    ClientId = "payment-client",
                    ClientName = "Client Credentials for Payment API",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("payment-secret".Sha256())
                    },

                    AllowedScopes =
                    {
                        "payment"
                    },

                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                };

                context.Clients.Add(client.ToEntity());
                context.SaveChanges();
            }
            #endregion

            #region MVC Client
            if (!context.Clients.Any(_ => _.ClientId == "mvc-client"))
            {
                var client = new Client
                {
                    ClientId = "mvc-client",
                    ClientSecrets = { new Secret("mvc-client-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = false,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5004/signin-oidc" },
                    
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5004/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "payment"
                    },

                    AllowOfflineAccess = true
                };

                context.Clients.Add(client.ToEntity());
                context.SaveChanges();
            }
            #endregion
        }

        public static void SeedResources(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.IdentityResources.Any())
            {
                var resources = new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email()
                };

                foreach (var resource in resources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }

        public static void SeedApiResource(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.ApiResources.Any())
            {
                var apiResource = new ApiResource("payment", "Payment API");
                context.ApiResources.Add(apiResource.ToEntity());
                context.SaveChanges();
            }
        }

        public static void SeedUser(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Users.Any(_ => _.Email == "admin@admin.com"))
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var user = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com", EmailConfirmed = true };
                var result = userManager.CreateAsync(user, "Abc@12345").Result;

                if (result.Succeeded)
                {
                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    _ = roleManager.CreateAsync(new IdentityRole { Name = "Master" }).Result;

                    user = userManager.FindByNameAsync("admin@admin.com").Result;

                    userManager.AddToRoleAsync(user, "Master");
                }
            }
        }
    }
}