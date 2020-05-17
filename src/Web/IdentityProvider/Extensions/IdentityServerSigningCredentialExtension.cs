using IdentityProvider.Configuration;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace IdentityProvider.Extensions
{
    public static class IdentityServerSigningCredentialExtension
    {
        public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder identityServerBuilder,
            IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            var appSettings = services.BuildServiceProvider()
                .GetService<IOptionsMonitor<AppSettings>>().CurrentValue;

            if (appSettings.UseAzure)
            {
                #region Uso do *.pfx direto

                if (webHostEnvironment.IsDevelopment())
                    identityServerBuilder.AddDeveloperSigningCredential();
                else
                    identityServerBuilder.AddSigningCredential(appSettings.DigitalCertificateThumbPrint, StoreLocation.CurrentUser, NameType.Thumbprint);
                #endregion

                return identityServerBuilder;
            }
            else if (!string.IsNullOrWhiteSpace(appSettings.DigitalCertificateThumbPrint))
            {
                #region Thumbprint
                if (webHostEnvironment.IsDevelopment())
                    identityServerBuilder.AddDeveloperSigningCredential();
                else
                    identityServerBuilder.AddSigningCredential(appSettings.DigitalCertificateThumbPrint, StoreLocation.CurrentUser, NameType.Thumbprint);
                #endregion

                return identityServerBuilder;
            }
            else if (!string.IsNullOrWhiteSpace(appSettings.DigitalCertificateRsaFileName))
            {
                #region Forma de uso maqueado
                if (webHostEnvironment.IsDevelopment())
                    identityServerBuilder.AddDeveloperSigningCredential();
                else
                    identityServerBuilder.AddCustomSecuritySigningCredential(services);
                #endregion
            }

            return identityServerBuilder;
        }
    }
}