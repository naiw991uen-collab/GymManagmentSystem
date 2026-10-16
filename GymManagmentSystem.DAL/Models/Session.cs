namespace GymManagmentSystem.DAL.Models;
public class Session : BaseEntity
{
    public string Discription { get; set; } = default!;
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Trainer Trainer { get; set; } = default!;
    public int TrainerId { get; set; }
    public Category Category { get; set; } = default!;
    public int CategoryId { get; set; }
    public ICollection<MemberSession> MemberSessions { get; set; } = default!;
}
