using AdminProject.Business.Managers;
using AdminProject.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminProject.Controllers.ApiControllers;

[Authorize(Roles = "Admin,UserAdmin")]
[ApiController]
[Route("api/user")]
public class UserApiController(
    UserAccountManager userManager,
    RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var users = await userManager.GetUsers();
        
        return Ok(users.Select(user => new UserListViewModel()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
        }));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles()
    {
        var roles = roleManager.Roles.ToList();
        
        return Ok(roles);
    }

    [HttpGet("claims")]
    public IActionResult Claims()
    {
        var claims = AccountClaimManager.GetAllClaims();
        
        return Ok(claims);
    }
}