using System.Security.Claims;
using AdminProject.Business.Managers;
using AdminProject.Data.Domain;
using AdminProject.Data.Domain.Users;
using AdminProject.Models.Exceptions;
using AdminProject.Models.Login;
using AdminProject.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminProject.Controllers;

public class UserController(
    UserAccountManager userAccountManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    [Authorize(Policy = AccountClaimManager.UserViewClaim)]
    public IActionResult List()
    {
        return View();
    }

    public IActionResult Roles()
    {
        return View();
    }

    [HttpGet("user/add")]
    public IActionResult AddUser()
    {
        return View(new UserAddViewModel());
    }

    [HttpPost("user/add")]
    public async Task<IActionResult> AddUser(UserAddViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var foundedUser = await userAccountManager.GetUser(model.Email);

        if (foundedUser is not null)
        {
            ModelState.AddModelError("email", "Mail adresi zaten kullanılıyor.");
            
            return View(model);
        }
        
        try
        {
            var createdUser = await userAccountManager.Save(new User()
            {
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.UserName
            }, model.Password, model.Roles);

            return RedirectToAction("EditUser", new { id = new Guid(createdUser.Id) });
        }
        catch (IdentityUserValidationException userValidationException)
        {
            foreach (var identityError in userValidationException.IdentityErrors)
            {
                ModelState.AddModelError("PasswordValidation", identityError);
            }
        }
        
        return View(model);
    }

    [HttpGet("user/edit/{id:guid}")]
    public async Task<IActionResult> EditUser(Guid? id)
    {
        if (id == null)
            return View();

        var user = await userAccountManager.GetUser(id.Value);

        if (user == null)
            return View("List");

        var roles = await userAccountManager.GetRoles(user.Id);
        
        return View(new UserEditViewModel()
        {
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Roles = roles.ToList()
        });
    }

    [HttpPost("user/edit")]
    public async Task<IActionResult> EditUser(UserEditViewModel model)
    {
        var foundedUser = await userAccountManager.GetUser(model.Email);

        if (foundedUser is null)
        {
            ModelState.AddModelError("email", "Kullanıcı bulunamadı.");
            
            return View(model);
        }
        
        try
        {
            var createdUser = await userAccountManager.Update(new User()
            {
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.UserName
            }, model.Password, model.Roles);

            return RedirectToAction("EditUser", new { id = new Guid(createdUser.Id) });
        }
        catch (IdentityUserValidationException userValidationException)
        {
            foreach (var identityError in userValidationException.IdentityErrors)
            {
                ModelState.AddModelError("PasswordValidation", identityError);
            }
        }
        
        return View(model);
    }

    [HttpGet("user/delete/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid? id)
    {
        if (!id.HasValue)
            return RedirectToAction("List");
        
        await userAccountManager.Delete(id.Value);
        
        return RedirectToAction("List");
    }

    [HttpGet("role/add")]
    public IActionResult AddRole()
    {
        return View(new RoleAddViewModel());
    }
    
    [HttpPost("role/add")]
    public async Task<IActionResult> AddRole(RoleAddViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await roleManager.RoleExistsAsync(model.RoleName))
        {
            ModelState.AddModelError("Name", "Benzer rol mevcut.");
            
            return View(model);
        }
        
        var result = await roleManager.CreateAsync(new IdentityRole(model.RoleName));

        if (!result.Succeeded)
        {
            ModelState.AddModelError("Name",string.Join(',', result.Errors.Select(p=>p.Description)));
        }
        
        var createdRole = await roleManager.FindByNameAsync(model.RoleName);

        foreach (var claim in model.Claims)
        {
            await roleManager.AddClaimAsync(createdRole, new Claim(ClaimType.Permission,claim));
        }
        
        return View(new RoleAddViewModel());
    }
    
    
    [HttpGet("role/edit/{id}")]
    public async Task<IActionResult> EditRole(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
            return RedirectToAction("Roles");

        var foundedRole = roleManager.Roles.AsNoTracking().FirstOrDefault(p => p.Id == id);

        if (foundedRole is null)
        {
            return RedirectToAction("Roles");
        }

        var claims = await roleManager.GetClaimsAsync(new IdentityRole()
        {
            Id = id,
            Name = foundedRole.Name
        });

        return View(new RoleEditViewModel()
        {
            Id = id,
            RoleName = foundedRole.Name,
            Claims = claims.Select(p=> p.Value).ToList()
        });
    }
    
    [HttpPost("role/edit/{id}")]
    public async Task<IActionResult> EditRole(RoleEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var role = await roleManager.FindByIdAsync(model.Id);

        if (role == null)
        {
            ModelState.AddModelError("Name", "Rol bulunamadı.");
            return View(model);
        }

        role.Name = model.RoleName;

        var result = await roleManager.UpdateAsync(role);
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError("Name",string.Join(',', result.Errors.Select(p=>p.Description)));
        }
        
        var foundedRole = await roleManager.FindByNameAsync(model.RoleName);
        var foundedClaims = await roleManager.GetClaimsAsync(foundedRole);

        foreach (var claim in model.Claims)
        {
            if(foundedClaims != null && foundedClaims.Any(p=>p.Value == claim))
               continue;
               
            await roleManager.AddClaimAsync(foundedRole, new Claim(ClaimType.Permission,claim));
        }
        
        return RedirectToAction("EditRole", new { id = model.Id });
    }
}