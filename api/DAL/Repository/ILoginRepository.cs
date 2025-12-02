using Microsoft.AspNetCore.Identity;

namespace HomeCareApp.DAL.Repository;
    //Repository for handling the login and logout 

public interface ILoginRepository
{
    Task<SignInResult> Login(string email, string password);
    Task Logout();
}