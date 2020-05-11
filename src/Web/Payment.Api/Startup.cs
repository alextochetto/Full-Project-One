using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payments.AntiCorruption.Facades;
using Payments.AntiCorruption.Gateways;
using Payments.Business.Interfaces;
using Payments.Business.Services;

namespace Payment.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("IdentityServerAuthentication:Authority").Value;
                    options.RequireHttpsMetadata = false;

                    options.ApiName = Configuration.GetSection("IdentityServerAuthentication:ApiName").Value;
                });

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICreditCardFacade, CreditCardFacade>();
            services.AddScoped<IDebitCardFacade, DebitCardFacade>();
            services.AddScoped<IPayPalGateway, PayPalGateway>();
            services.AddScoped<IEloGateway, EloGateway>();

            services.AddControllers();
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
        }
    }
}
