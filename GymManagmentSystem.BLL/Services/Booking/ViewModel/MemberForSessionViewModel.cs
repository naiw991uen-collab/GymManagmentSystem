namespace GymManagmentSystem.BLL.Services.Booking.ViewModel;
public class MemberForSessionViewModel
{
    public int MemberId { get; set; }
    public int SessionId { get; set; }
    public string MemberName { get; set; } = default!;
    public string BookingDays { get; set; } = default!;
    public bool IsAttend { get; set; }
}
