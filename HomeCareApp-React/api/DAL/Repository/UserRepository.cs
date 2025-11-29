using Microsoft.AspNetCore.Identity;
using HomeCareApp.Models;

namespace HomeCareApp.DAL;

public class UserRepository : IUserRepository
{
    private readonly UserManager<Models.User> _userManager;
    private readonly SignInManager<Models.User> _signInManager;

    public UserRepository(UserManager<Models.User> userManager, SignInManager<Models.User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public Task<Models.User?> FindByEmailAsync(string email)
        => _userManager.FindByEmailAsync(email);

    public Task<SignInResult> PasswordSignInAsync(Models.User user, string password)
        => _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

    public Task SignOutAsync()
        => _signInManager.SignOutAsync();

    public Task<IdentityResult> CreateAsync(Models.User user, string password)
        => _userManager.CreateAsync(user, password);

    public Task<IdentityResult> AddToRoleAsync(Models.User user, string role)
        => _userManager.AddToRoleAsync(user, role);

    public async Task<Models.User?> GetUserAsync(System.Security.Claims.ClaimsPrincipal user)
        => await _userManager.GetUserAsync(user);

    public async Task<List<Models.User>> GetUsersInRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.ToList();
    }
}