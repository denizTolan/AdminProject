using System.Security.Claims;
using AdminProject.Data.Domain.Users;
using AdminProject.Data.Repositories;
using AdminProject.Models.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminProject.Business.Managers;

public class UserAccountManager(
    IRepository<User,string> userRepository,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await userRepository.GetAll().ToListAsync();
    }

    public async Task<User> GetUser(Guid userId)
    {
        return await userRepository.GetById(userId.ToString());
    }

    public async Task<User> GetUser(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<User> Save(User user,string password,IEnumerable<string> roles)
    {
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new IdentityUserValidationException(result.Errors.Select(p=>p.Description));
        }
        
        var createdUser = await userManager.FindByEmailAsync(user.Email);

        await userManager.AddToRolesAsync(user, roles);
            
        return createdUser;
    }

    public async Task<User> Update(User user, string password, IEnumerable<string> roles)
    {
        var foundedUser = await userManager.FindByEmailAsync(user.Email);

        if (foundedUser == null)
        {
            throw new IdentityUserValidationException(["User not found"]);
        }

        var result = new IdentityResult();

        if (!string.IsNullOrEmpty(password))
        {
            result = await userManager.ChangePasswordAsync(user, foundedUser.PasswordHash  ,password);
        
            if (!result.Succeeded)
            {
                throw new IdentityUserValidationException(result.Errors.Select(p=>p.Description));
            }
        }

        var userRoles =await userManager.GetRolesAsync(foundedUser);
        await userManager.RemoveFromRolesAsync(foundedUser, userRoles);

        result = await userManager.AddToRolesAsync(foundedUser, roles);
        
        if (!result.Succeeded)
        {
            throw new IdentityUserValidationException(result.Errors.Select(p=>p.Description));
        }
            
        return foundedUser;
    }

    public async Task Delete(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        await userManager.DeleteAsync(user);
    }

    public async Task<IEnumerable<string>> GetRoles(string userId)
    {
        var foundedUser = await userManager.FindByIdAsync(userId);

        if (foundedUser == null)
        {
            throw new IdentityUserValidationException(["User not found"]);
        }
        
        return await userManager.GetRolesAsync(foundedUser);
    }
}