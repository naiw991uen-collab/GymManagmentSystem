namespace GymManagmentSystem.DAL.Models;
public class MemberSession : BaseEntity
{
    public Member Member { get; set; } = default!;
    public int MemberId { get; set; }
    public Session Session { get; set; } = default!;
    public int SessionId { get; set; }
    public int BookingDays { get; set; }
    public bool IsAttended { get; set; }
}
