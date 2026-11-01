using System.ComponentModel.DataAnnotations;

namespace GymManagmentSystem.BLL.Services.Sessions.ViewModel;
public class CreateSessionViewModel
{
    public string Description { get; set; } = default!;
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TrainerId { get; set; }
    public int CategoryId { get; set; }
}
