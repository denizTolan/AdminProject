using System.Security.Claims;
using AdminProject.Models.Layout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace AdminProject.ViewComponents;

[ViewComponent(Name = "Menu")]
public class MenuViewComponent : ViewComponent
{
    public ViewViewComponentResult Invoke()
    {
        var userRoles = HttpContext?.User?.Claims?.Where(c => c.Type == ClaimTypes.Role)?.Select(p=> p.Value)?.ToList();

        var menuItems = new List<MenuItemViewModel>
        {
            new MenuItemViewModel
            {
                Title = "Kullanıcı ve Roller",
                Icon = "fas fa-tachometer-alt",
                RequiredRole = "Admin",  // Sadece Admin görebilir
                SubMenus = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Title = "Kullanıcı Listesi", Url = "/user/list", Icon = "far fa-circle", RequiredRole = "Admin" },
                    new MenuItemViewModel { Title = "Roller", Url = "/user/roles", Icon = "far fa-circle", RequiredRole = "Admin" }
                }
            }
        };

        // Kullanıcının sahip olmadığı rolleri filtrele
        var filteredMenu = menuItems
            .Where(menu => string.IsNullOrEmpty(menu.RequiredRole) 
                           || (userRoles != null && userRoles.Contains(menu.RequiredRole))
            )
            .Select(menu => new MenuItemViewModel
            {
                Title = menu.Title,
                Url = menu.Url,
                Icon = menu.Icon,
                SubMenus = menu.SubMenus
                    .Where(sub => string.IsNullOrEmpty(sub.RequiredRole)
                                  || (userRoles != null && userRoles.Contains(menu.RequiredRole)))
                    .ToList()
            })
            .Where(menu => menu.SubMenus.Any() || !string.IsNullOrEmpty(menu.Url)) // Boş menüleri kaldır
            .ToList();

        return View(filteredMenu);
    }
}