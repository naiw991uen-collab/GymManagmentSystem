namespace GymManagmentSystem.BLL.Services.Trainers.ViewModel;
public class TrainerDetailsViewModel
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? DateOfBirth { get; set; }
    public string Address { get; set; } = default!;
    public string Specialty { get; set; } = default!;
}
