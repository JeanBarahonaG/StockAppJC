using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StockAppJC.Models;
using StockAppJC.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockAppJC.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IClientStore _clientStore;
        private readonly IConfiguration _configuration;

        public TokenService(UserManager<Usuario> userManager, IClientStore clientStore, IConfiguration configuration)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            _configuration = configuration;
        }

        public async Task<string> GenerateTokenAsync(Usuario user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, user.Nombre),
                new Claim(JwtClaimTypes.FamilyName, user.Apellido),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim(JwtClaimTypes.IdentityProvider, "Local")
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            return await TokenGenerate(claims, secretKey);
        }

        private async Task<string> TokenGenerate(List<Claim> claims, SymmetricSecurityKey secretKey)
        {
            var claimsIdentity = new ClaimsIdentity(claims, "Authentication");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var client = await _clientStore.FindClientByIdAsync("StockAppJC");

            if (client == null)
            {
                throw new InvalidOperationException("Client not Found");
            }

            var validatedResources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiResources = Config.GetApiResources().ToList(),
                    ApiScopes = Config.GetApiScopes().ToList(),
                    IdentityResources = Config.GetIdentityResources().ToList()
                }
            };

            var validadRequest = new ValidatedRequest
            {
                Client = client,
                Subject = claimsPrincipal,
                ClientId = client.ClientId,
                ValidatedResources = validatedResources,
                ClientClaims = claims,
                Secret = new ParsedSecret
                {
                    Id = client.ClientId,
                    Credential = client.ClientSecrets.FirstOrDefault()?.Value,
                    Type = "SharedSecret"
                }
            };

            var tokenCreationRequest = new TokenCreationRequest
            {
                Subject = claimsPrincipal,
                ValidatedRequest = validadRequest,
                ValidatedResources = validatedResources
            };

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        public async Task<bool> TokenValidation(string token)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken
                    (token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
