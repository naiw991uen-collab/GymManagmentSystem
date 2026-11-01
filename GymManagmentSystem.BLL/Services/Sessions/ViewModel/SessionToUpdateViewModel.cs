using System.ComponentModel.DataAnnotations;

namespace GymManagmentSystem.BLL.Services.Sessions.ViewModel;
public class SessionToUpdateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Trainer is required")]
    [Display(Name = "Trainer")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 5)]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date & Time")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date & Time")]
    public DateTime EndDate { get; set; }
}
