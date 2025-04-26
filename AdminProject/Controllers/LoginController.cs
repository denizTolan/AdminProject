using AdminProject.Data.Domain.Users;
using AdminProject.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminProject.Controllers;

[AllowAnonymous]
[Route("[controller]")]
public class LoginController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("singin")]
    public async Task<IActionResult> SignIn(LoginUserViewModel model)
    {
        var foundedUser = await _userManager.FindByEmailAsync(model.Email);
        if (foundedUser == null)
        {
            return RedirectToAction("Index");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(foundedUser, model.Password, false, false);

        if (!signInResult.Succeeded)
        {
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index", "Home");
    }
}