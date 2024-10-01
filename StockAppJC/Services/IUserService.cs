using StockAppJC.General;
using StockAppJC.Models.ViewModels;

namespace StockAppJC.Services
{
    public interface IUserService
    {
        Task<answer> RegisterUser(RegisterUserViewModel user);
        Task<answer> UpdateUser(string id , UpdateUserViewModel user);
        Task<answer> DeleteUser(HttpContext httpContext, string id);
        Task<UserViewModel> GetUser(string id);
        Task<IEnumerable<UserViewModel>> GetUsers();
        Task<answer> UpdatePassword(string id ,UpdatePasswordUserViewModel password);
    }
}
