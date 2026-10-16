namespace GymManagmentSystem.DAL.Models;
public class Category : BaseEntity
{
    public string CategoryName { get; set; } = default!;
    public ICollection<Session> Sessions { get; set; } = default!;
}
