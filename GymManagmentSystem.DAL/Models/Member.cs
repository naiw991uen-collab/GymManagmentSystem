namespace GymManagmentSystem.DAL.Models;
public class Member : GymUser
{
    // JoinDate => CreatedAt
    public string? Photo { get; set; }

    #region Relationships
    public HealthRecoed HealthRecoed { get; set; } = default!;
    public ICollection<MemberShip> MemberShips { get; set; } = default!;
    public ICollection<MemberSession> MemberSessions { get; set; } = default!; 
    #endregion

}
