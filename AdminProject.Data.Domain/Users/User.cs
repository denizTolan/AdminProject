using Microsoft.AspNetCore.Identity;

namespace AdminProject.Data.Domain.Users;

public class User:IdentityUser, IEntity<string>
{
}