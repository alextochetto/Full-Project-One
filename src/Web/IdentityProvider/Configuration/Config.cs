using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityProvider.Configuration
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("payment", "Payment API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "payment-client",
                    ClientName = "Client Credentials for Payment API",
                    ClientSecrets = { new Secret("payment-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "payment" }
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc-client",
                    ClientSecrets = { new Secret("mvc-client-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = true,
                
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
                }
            };
    }
}