using Microsoft.AspNetCore.Identity;
using HomeCareApp.Models;

namespace HomeCareApp.DAL
{ 
    //repository for managing user authentication operations
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserRepository> _logger; // logger

        public UserRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; // injected logger
        }
        //filters user by email
        public async Task<User?> FindByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Finding user by email {Email}", email); // info
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by email {Email}", email); // error
                throw;
            }
        }
        //handles sign in for user
        public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        {
            try
            {
                _logger.LogInformation("Password sign-in attempt for {Email}", user.Email); // info
                return await _signInManager.PasswordSignInAsync(
                    user, password, isPersistent: false, lockoutOnFailure: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password sign-in for {Email}", user.Email); // error
                throw;
            }
        }
        //Sign out the current user
        public async Task SignOutAsync()
        {
            try
            {
                _logger.LogInformation("Signing out current user"); // info
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign-out"); // error
                throw;
            }
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            try
            {
                _logger.LogInformation("Creating new user {Email}", user.Email); // info
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                    _logger.LogWarning("User creation failed for {Email}", user.Email); // warning

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", user.Email); // error
                throw;
            }
        } 
        //Gives a role to user

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            try
            {
                _logger.LogInformation("Adding user {Email} to role {Role}", user.Email, role); // info
                var result = await _userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                    _logger.LogWarning("Failed to add user {Email} to role {Role}", user.Email, role); // warning

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {Email} to role {Role}", user.Email, role); // error
                throw;
            }
        }

        public async Task<User?> GetUserAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            try
            {
                _logger.LogInformation("Fetching authenticated user from ClaimsPrincipal"); // info
                return await _userManager.GetUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user from ClaimsPrincipal"); // error
                throw;
            }
        }

        public async Task<List<User>> GetUsersInRoleAsync(string role)
        {
            try
            {
                _logger.LogInformation("Fetching users in role {Role}", role); // info

                var users = await _userManager.GetUsersInRoleAsync(role);

                _logger.LogInformation("Loaded {Count} users in role {Role}", users.Count, role); // info

                return users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users in role {Role}", role); // error
                throw;
            }
        }
    }
}
