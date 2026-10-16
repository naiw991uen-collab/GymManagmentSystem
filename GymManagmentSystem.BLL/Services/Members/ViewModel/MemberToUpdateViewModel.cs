namespace GymManagmentSystem.BLL.Services.Members.ViewModel;
public class MemberToUpdateViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Photo { get; set; }
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public int BuildingNumber { get; set; }
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
}
