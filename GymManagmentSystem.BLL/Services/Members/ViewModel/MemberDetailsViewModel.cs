namespace GymManagmentSystem.BLL.Services.Members.ViewModel;
public class MemberDetailsViewModel
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string? Photo { get; set; }
    public string? PlanName { get; set; }
    public string? MembershipStartDate { get; set; }
    public string? MembershipEndDate { get; set; }
    public string? DateOfBirth { get; set; }
    public string Address { get; set; } = default!;
}
