using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            //.AddAspNetIdentity<ApplicationUser>();

            identityServer.AddInMemoryIdentityResources(_configuration.GetSection(nameof(IdentityResources)));
            identityServer.AddInMemoryApiResources(_configuration.GetSection(nameof(ApiResource)));
            identityServer.AddInMemoryClients(_configuration.GetSection(nameof(Client)));

            if (_webHostEnvironment.IsDevelopment())
                identityServer.AddDeveloperSigningCredential();
            //else
            //    identityServer.AddSigningCredential("6AA95D1EA9DFD6B5E4029D165DBA499D8DC1BB50", StoreLocation.CurrentUser, NameType.Thumbprint);

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