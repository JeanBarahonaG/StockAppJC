using StockAppJC.Models;

namespace StockAppJC.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(Usuario user);

        Task<bool> TokenValidation(string token);
    }
}
