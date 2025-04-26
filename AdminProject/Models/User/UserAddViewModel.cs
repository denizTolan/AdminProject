using System.ComponentModel.DataAnnotations;
using AdminProject.Validators;

namespace AdminProject.Models.User;

public class UserAddViewModel
{
    public string? UserId { get; set; }
    
    [Required(ErrorMessage = "Kullanıcı Adı Zorunlu")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Mail Zorunlu")]
    [EmailAddress(ErrorMessage = "Mail Formatı Yanlış")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Şire Zorunlu")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalı")]
    public string Password { get; set; }
    
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    public string PasswordCheck { get; set; }
    
    [Required(ErrorMessage = "En az bir rol seçilmelidir.")]
    [MinRoles(1, ErrorMessage = "En az bir rol seçilmelidir.")]
    public List<string> Roles { get; set; } = new List<string>();
}