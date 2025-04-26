using System.ComponentModel.DataAnnotations;
using AdminProject.Validators;

namespace AdminProject.Models.User;

public class RoleEditViewModel
{
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Rol Adı Zorunlu")]
    public string RoleName { get; set; }
    
    [Required(ErrorMessage = "En az bir izin seçilmelidir.")]
    [MinRoles(1, ErrorMessage = "En az bir izin seçilmelidir.")]
    public List<string> Claims { get; set; } = new List<string>();
}