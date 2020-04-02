using IdentityServer4;
using IdentityServer4.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace IdentityProvider.Extensions
{
    public static class IdentityServerBuilderCryptoExtension
    {
        private class TemporaryRsaKey
        {
            public string KeyId { get; set; }
            public RSAParameters Parameters { get; set; }
        }

        private class RsaKeyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                property.Ignored = false;

                return property;
            }
        }

        public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder)
        {
            var signingAlgorithm = IdentityServerConstants.RsaSigningAlgorithm.RS256;
            var rsaSecurityKey = CryptoHelper.CreateRsaSecurityKey();

            var temporaryRsaKey = new TemporaryRsaKey
            {
                KeyId = rsaSecurityKey.KeyId
            };

            if (rsaSecurityKey.Rsa is null)
                temporaryRsaKey.Parameters = rsaSecurityKey.Parameters;
            else
                temporaryRsaKey.Parameters = rsaSecurityKey.Rsa.ExportParameters(includePrivateParameters: true);

            var filename = Path.Combine(Directory.GetCurrentDirectory(), "tempkey.rsa");

            if (File.Exists(filename))
            {
                var keyFile = File.ReadAllText(filename);
                temporaryRsaKey = JsonConvert.DeserializeObject<TemporaryRsaKey>(keyFile, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

                rsaSecurityKey = CryptoHelper.CreateRsaSecurityKey(temporaryRsaKey.Parameters, temporaryRsaKey.KeyId);
            }
            else
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(temporaryRsaKey, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() }));
            }

            builder.AddSigningCredential(rsaSecurityKey, signingAlgorithm);

            return builder;
        }
    }
}