using GymManagmentSystem.DAL.Models.Enums;

namespace GymManagmentSystem.BLL.Services.Trainers.ViewModel;
public class CreateTrainerViewModel
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public int BuildingNumber { get; set; }
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public Specialty Specialty { get; set; }
}
