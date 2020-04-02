using IdentityProvider.Data;
using IdentityProvider.Models;
using IdentityProvider.Seeds;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace IdentityProvider
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var identityServer = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            #region Thumbprint
            if (_webHostEnvironment.IsDevelopment())
                identityServer.AddDeveloperSigningCredential();
            else
                identityServer.AddSigningCredential("4DFF9B8EBB5314B9A62EFA72DA8B4D7658231C05", StoreLocation.CurrentUser, NameType.Thumbprint);
            #endregion

            #region Uso do *.pfx direto
            //if (_webHostEnvironment.IsDevelopment())
            //    identityServer.AddDeveloperSigningCredential();
            //else
            //    identityServer.AddSigningCredential(new X509Certificate2("certificate.pfx", "987654321")); 
            #endregion

            #region Forma de uso maqueado
            //if (_webHostEnvironment.IsDevelopment())
            //    identityServer.AddDeveloperSigningCredential();
            //else
            //    identityServer.AddCustomSigningCredential();
            #endregion

            #region Identity configuration
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

            //.AddUserStore<ApplicationUser>()
            //        .AddRoleStore<IdentityRole>()
            //        .AddRoleManager<>();

            // esta configuração é para o token gerado e enviado para o usuário para recuperar a senha e para confirmar o e-mail do seu cadastro
            // o token é enviado por e-mail para os dois casos
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(1));
            #endregion

            #region IdentityServer configuration
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            identityServer.AddConfigurationStore(options =>
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

            services.AddAuthentication();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.RunMigration();

            app.SeeClient();
            app.SeedResources();
            app.SeedApiResource();

            app.SeedUser();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthentication(); // UseAuthentication not needed -- UseIdentityServer add this
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}