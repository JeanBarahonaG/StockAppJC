using System.Collections.Concurrent;

namespace StockAppJC.Token
{
    public class TokenRevocationManager
    {
        private static readonly ConcurrentDictionary<string, DateTime> RevokedTokens = new ConcurrentDictionary<string, DateTime>();

        public static void RevokeToken(string token)
        {
            RevokedTokens[token] = DateTime.UtcNow;
        }

        public static bool IsTokenRevoked(string token)
        {
            return RevokedTokens.ContainsKey(token);
        }
    }
}
