using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StockAppJC.Token
{
    public class TemporarySingingCredentialStore : ISigningCredentialStore
    {
        private readonly IConfiguration _configuration;

        public TemporarySingingCredentialStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            return Task.FromResult(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        }
    }
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "User roles", new List<string> { "role" })
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("StockAppJC", "StockAppJC API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "StockAppJC",
                    AccessTokenLifetime = 300, // Tiempo de vida del token de acceso en segundos
                    AuthorizationCodeLifetime = 300, // Tiempo de vida del código de autorización en segundos
                    IdentityTokenLifetime = 300, // Tiempo de vida del token de identidad en segundos
                    AbsoluteRefreshTokenLifetime = 2592000, // Tiempo de vida absoluto del token de actualización en segundos
                    SlidingRefreshTokenLifetime = 1296000, // Tiempo de vida deslizante del token de actualización en segundos
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:StockAppJC:ClientSecret"].Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string> 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.AuthenticationTime,
                        "StockAppJC", "roles"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,

                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("StockAppJC", "StockAppJC API")
                {
                    Scopes = { "StockAppJC" },
                    UserClaims = { "role" }
                }
            };
        }

    }
}
