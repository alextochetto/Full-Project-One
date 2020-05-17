using Microsoft.Extensions.DependencyInjection;

namespace IdentityProvider.Extensions
{
    public static class IdentityServerConfigurationExtension
    {
        public static IIdentityServerBuilder AddIdentityServerConfiguration(this IServiceCollection services)
        {
            return services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });
        }
    }
}
