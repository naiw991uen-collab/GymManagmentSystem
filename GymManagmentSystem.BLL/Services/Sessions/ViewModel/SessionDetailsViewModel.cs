namespace GymManagmentSystem.BLL.Services.Sessions.ViewModel;
public class SessionDetailsViewModel
{
    public int Id { get; set; }
    public string Description { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public string TrainerName { get; set; } = default!;
    public int Capacity { get; set; }
    public int AvailableSlots { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BookedCount => Capacity - AvailableSlots;
    public string CapacityDisplay => $"{BookedCount} / {Capacity} spots";
    public TimeSpan Duration => EndDate - StartDate;
    public string Status
    {
        get
        {
            if (StartDate > DateTime.Now)
                return "Upcoming";
            else if (StartDate <= DateTime.Now && EndDate >= DateTime.Now)
                return "Ongoing";
            else
                return "Completed";
        }
    }
}
