namespace AdminProject.Business.Managers;

public class AccountClaimManager
{
    public const string AdminClaim = "Admin";
    public const string UserViewClaim = "UserView";
    public const string UserAddClaim = "UserAdd";
    public const string UserEditClaim = "UserEdit";
    public const string UserDeleteClaim = "UserDelete";
    
    public static List<string> GetAllClaims()
    {
        return
        [
            AdminClaim,
            UserViewClaim,
            UserAddClaim,
            UserEditClaim,
            UserDeleteClaim
        ];
    }
}