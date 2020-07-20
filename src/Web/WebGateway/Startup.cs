using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace WebGateway
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = Configuration.GetSection("IdentityServerAuthentication:AuthenticationProviderKey").Value;
            Action<IdentityServerAuthenticationOptions> options = o =>
            {
                o.Authority = Configuration.GetSection("IdentityServerAuthentication:Authority").Value;
                o.ApiName = Configuration.GetSection("IdentityServerAuthentication:ApiName").Value;
                o.SupportedTokens = SupportedTokens.Both;
                o.RequireHttpsMetadata = false;
            };

            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, options);
            
            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOcelot().Wait();
        }
    }
}