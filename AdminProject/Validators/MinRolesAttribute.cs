using System.ComponentModel.DataAnnotations;
using AdminProject.Models.User;

namespace AdminProject.Validators;
    
public class MinRolesAttribute : ValidationAttribute
{
    private readonly int _minCount;

    public MinRolesAttribute(int minCount)
    {
        _minCount = minCount;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var roles = value as List<string>;
        if (roles == null || roles.Count < _minCount)
        {
            return new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }
}

