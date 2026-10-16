using System.ComponentModel.DataAnnotations;

namespace GymManagmentSystem.BLL.Services.Members.ViewModel;
public class HealthRecordViewModel
{
    public decimal Height { get; set; } = default!;

    public decimal Weight { get; set; } = default!;
    public string BloodType { get; set; } = default!;
    public string? Note { get; set; } = default!;
}
