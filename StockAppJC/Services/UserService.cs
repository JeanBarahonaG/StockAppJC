using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockAppJC.DbContext;
using StockAppJC.General;
using StockAppJC.Models;
using StockAppJC.Models.ViewModels;

namespace StockAppJC.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UserService(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserViewModel> GetUser(string id)
        {
            answer response = new answer();
            try 
            {
                var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) 
                {
                    throw new KeyNotFoundException("User Not Found");
                }

                var UserRol = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == UserRol.RoleId);

                UserViewModel userObj = new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Nombre,
                    LastName = user.Apellido,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = role.Name
                };

                return userObj;
            }
            catch (Exception ex) 
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            try
            {
                var users = _context.Usuarios.ToListAsync();
                List<UserViewModel> listUsers = new List<UserViewModel>();

                if (users.Result.ToList().Count > 0)
                {
                    foreach (var user in users.Result.ToList())
                    {
                        var UserRol = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        if (UserRol == null)
                            continue;

                        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == UserRol.RoleId);
                        if (role == null)
                            continue;

                        UserViewModel userObj = new UserViewModel
                        {
                            Id = user.Id,
                            Name = user.Nombre,
                            LastName = user.Apellido,
                            Username = user.UserName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Role = role.Name
                        };
                        listUsers.Add(userObj);
                    }
                }
                return listUsers;
            }
            catch (Exception ex)
            {
                return new List<UserViewModel>();
            }
        }
        public async Task<answer> RegisterUser(RegisterUserViewModel user)
        {
            answer response = new answer();
            try 
            {
                var newUser = new Usuario
                {
                    Nombre = user.Name,
                    Apellido = user.LastName,
                    UserName = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    var userRole = await _userManager.AddToRoleAsync(newUser, user.Role);
                    response.code = 200;
                    response.description = "User Created";
                }
                else
                {
                    response.code = 400;
                    response.description = "User Not Created";
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
            return response;
        }

        public async Task<answer> UpdateUser(string id, UpdateUserViewModel user)
        {
            answer response = new answer();
            try 
            {
                var userUpdate = await _userManager.FindByIdAsync(id);
                if (userUpdate == null)
                {
                    response.code = 404;
                    response.description = "User Not Found";
                }
                userUpdate.Nombre = user.Name;
                userUpdate.Apellido = user.LastName;
                userUpdate.UserName = user.Username;
                userUpdate.Email = user.Email;
                userUpdate.PhoneNumber = user.PhoneNumber;

                var currentRole = await _userManager.GetRolesAsync(userUpdate);

                if (!currentRole.Contains(user.Role)) 
                {
                    var revomeRole = await _userManager.RemoveFromRolesAsync(userUpdate, currentRole.ToArray());
                    if (!revomeRole.Succeeded) 
                    {
                        response.code = 400;
                        response.description = "User Not Updated";
                        return response;
                    }

                    var newRole = await _userManager.AddToRoleAsync(userUpdate, user.Role);
                    if (!newRole.Succeeded)
                    {
                        response.code = 400;
                        response.description = "User Not Updated";
                        return response;
                    }
                }
                var update = await _userManager.UpdateAsync(userUpdate);
                if (update.Succeeded)
                {
                    response.code = 200;
                    response.description = "User Updated";
                    response.result = "User Update Succeeded";
                }
                else
                {
                    response.code = 400;
                    response.description = "User Not Updated";
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Internal Server Error: {ex.Message}");
            }
            return response;
        }
        public async Task<answer> DeleteUser(HttpContext httpContext, string id)
        {
            answer response = new answer();
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    response.code = 404;
                    response.description = "User Not Found";
                    return response;
                }

                var role = await _userManager.GetRolesAsync(user);
                if (role != null && role.Any())
                {
                    var removeRole = await _userManager.RemoveFromRolesAsync(user, role);
                    if (!removeRole.Succeeded)
                    {
                        response.code = 400;
                        response.description = "User Not Deleted";
                        return response;
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    response.code = 200;
                    response.description = "User Deleted";
                }
                else
                {
                    response.code = 400;
                    response.description = "User Not Deleted";
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.description = $"Internal Server Error: {ex.Message}";
            }
            return response;
        }
        public async Task<answer> UpdatePassword(string id, UpdatePasswordUserViewModel password)
        {
            answer response = new answer();
            try 
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.code = 404;
                    response.description = "User Not Found";
                    return response;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, password.Password);

                if (result.Succeeded)
                {
                    response.code = 200;
                    response.description = "Password Updated";
                }
                else
                {
                    response.code = 400;
                    response.description = "Password Not Updated";
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.description = $"Internal Server Error: {ex.Message}";
            }
            return response;
        }
    }
}
