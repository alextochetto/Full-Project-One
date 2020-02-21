using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.Extensions;

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
            //identityServer.AddInMemoryIdentityResources(Configuration.Config.Ids);
            //identityServer.AddInMemoryApiResources(Configuration.Config.Apis);
            //identityServer.AddInMemoryClients(Configuration.Config.Clients);

            identityServer.AddInMemoryIdentityResources(_configuration.GetSection(nameof(IdentityResources)));
            identityServer.AddInMemoryApiResources(_configuration.GetSection(nameof(ApiResource)));
            identityServer.AddInMemoryClients(_configuration.GetSection(nameof(Client)));

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

            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}