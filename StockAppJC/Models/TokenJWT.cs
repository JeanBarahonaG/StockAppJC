using StockAppJC.DbContext;
using StockAppJC.General;
using System.Security.Claims;

namespace StockAppJC.Models
{
    public class TokenJWT
    {
        private readonly ApplicationDbContext _context;

        public TokenJWT()
        {
        }
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        public answer tokenValidation(ClaimsIdentity claims)
        {
            answer response = new answer();
            try
            {
                if (claims == null || claims.Claims.Count() == 0)
                {
                    response.code = 401;
                    response.description = "Token is invalid due Claims is null";
                    response.result = "Token is invalid";
                    return response;
                }

                var id = claims.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    response.code = 401;
                    response.description = "Token is invalid due ID is null";
                    response.result = "Token is invalid";
                    return response;
                }

                Usuario user = _context.Usuarios.FirstOrDefault(x => x.Id == id);

                if (user == null)
                {
                    response.code = 401;
                    response.description = "Token is invalid due User is null";
                    response.result = "Token is invalid";
                    return response;
                }  
                
                response.code = 200;
                response.description = "Token is valid";
                response.result = user.Id;
                return response;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.description = ex.Message;
                response.result = "Token is invalid";
                return response;

            }
        }
    }
}
