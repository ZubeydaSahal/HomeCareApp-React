using Microsoft.AspNetCore.Identity;

namespace HomeCareApp.DAL;

public interface IUserRepository
{
    //Repository interface for managing users
    Task<Models.User?> FindByEmailAsync(string email);
    Task<SignInResult> PasswordSignInAsync(Models.User user, string password);
    Task SignOutAsync();
    Task<IdentityResult> CreateAsync(Models.User user, string password);
    Task<IdentityResult> AddToRoleAsync(Models.User user, string role);
    Task<Models.User?> GetUserAsync(System.Security.Claims.ClaimsPrincipal user);
    Task<List<Models.User>> GetUsersInRoleAsync(string role);
}