using GymManagmentSystem.DAL.Models.Enums;

namespace GymManagmentSystem.BLL.Services.Trainers.ViewModel;
public class TrainerToUpdateViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; } 
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public int BuildingNumber { get; set; }
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public Specialty Specialty { get; set; }
}
