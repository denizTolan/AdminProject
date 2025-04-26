namespace AdminProject.Models.Exceptions;

public class IdentityUserValidationException:Exception
{
    public IEnumerable<string> IdentityErrors { get; set; }

    public IdentityUserValidationException(IEnumerable<string> identityErrors)
    {
        this.IdentityErrors = identityErrors;
    }
}