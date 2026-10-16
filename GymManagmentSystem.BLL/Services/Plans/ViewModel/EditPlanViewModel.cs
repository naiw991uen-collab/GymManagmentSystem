using System.ComponentModel.DataAnnotations;

namespace GymManagmentSystem.BLL.Services.Plansa.ViewModel;
public class EditPlanViewModel
{
    public int Id { get; set; }
    public string? PlanName { get; set; }
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
}
