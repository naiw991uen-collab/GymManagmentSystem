using GymManagmentSystem.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;
namespace GymManagmentSystem.BLL.Services.Members.ViewModel;
public class CreateMemberViewModel
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public int BuildingNumber { get; set; }
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public HealthRecordViewModel HealthRecordViewModel { get; set; } = default!;
}
