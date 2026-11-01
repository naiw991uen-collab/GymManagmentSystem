using Microsoft.AspNetCore.Identity;

namespace GymManagmentSystem.DAL.Applications;
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}
