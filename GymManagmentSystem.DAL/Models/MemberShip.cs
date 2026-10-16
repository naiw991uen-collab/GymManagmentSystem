namespace GymManagmentSystem.DAL.Models;

public class MemberShip : BaseEntity
{
    public Member Member { get; set; } = default!;
    public int MemberId { get; set; }
    public Plan Plan { get; set; } = default!;
    public int PlanId { get; set; }

    // StartDate => CreatedAt
    public DateTime EndDate { get; set; }
    public string Status => IsActive ? "Active" : "Expired";

    public bool IsActive => EndDate > DateTime.Now && CreatedAt <= DateTime.Now;
}
